using System.Collections.Concurrent;
using Chunks.MarchingCubes.MeshGenerators;
using Chunks.Voxels;
using NLog;
using OpenTK.Mathematics;
using Physics.Collisions;

namespace Chunks.ChunkManagement.ChunkWorkers;

public class ChunkWorker : IChunkWorker
{
    private bool _isUpdatingUnlocked;

    private readonly object _lockObj = new();

    public bool IsUpdating
    {
        get
        {
            lock (_lockObj)
                return _isUpdatingUnlocked;
        }
        private set
        {
            lock (_lockObj)
                _isUpdatingUnlocked = value;
        }
    }

    private enum JobType
    {
        Load,
        Save,
        Update
    }

    private readonly BlockingCollection<JobType> _jobs = new(new ConcurrentQueue<JobType>());

    public List<Chunk> Chunks { get; }

    private readonly HashSet<Vector3i> _existingChunks = new();

    private readonly ConcurrentQueue<Vector3i> _chunksToLoad = new();

    private readonly ConcurrentQueue<Chunk> _loadedChunks = new();

    private readonly ConcurrentQueue<Vector3i> _chunksToSaveQueue = new();

    private readonly ConcurrentDictionary<Vector3i, Voxel[,,]> _chunksToSaveDictionary = new();

    private readonly HashSet<Vector3i> _savedChunks = new();

    private readonly ConcurrentQueue<Chunk> _chunksToUpdateQueue = new();

    private readonly ConcurrentHashSet<Chunk> _chunksToUpdateHashSet = new();

    private readonly ConcurrentQueue<Chunk> _updatedChunks = new();

    private readonly SimulationManager<PoseIntegratorCallbacks> _simulationManager;

    private readonly ChunkFactory _chunkFactory;

    private readonly int _renderDistance;

    private const int NumberOfThreads = 2;

    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    private readonly ChunkHandler _chunkHandler;

    private int TotalChunks => (2 * _renderDistance + 1) * (2 * _renderDistance + 1) * (2 * _renderDistance + 1);

    private readonly CancellationTokenSource _cancellationTokenSource = new();

    private readonly MeshGenerator _meshGenerator;

    public ChunkWorker(List<Chunk> chunks, SimulationManager<PoseIntegratorCallbacks> simulationManager, ChunkFactory chunkFactory, ChunkHandler chunkHandler, MeshGenerator meshGenerator, int renderDistance)
    {
        Chunks = chunks;
        _simulationManager = simulationManager;
        _chunkFactory = chunkFactory;
        _chunkHandler = chunkHandler;
        _renderDistance = renderDistance;
        _meshGenerator = meshGenerator;
        foreach (var chunk in Chunks)
        {
            _existingChunks.Add(chunk.Position / Chunk.Size);
        }

        Start();
    }

    private void Start()
    {
        for (int i = 0; i < NumberOfThreads; i++)
        {
            Task.Run(RunJobs);
        }

        GetSavedChunks();
        EnqueueLoadingChunks(Vector3i.Zero);
        int prevNumber = 0;
        while (Chunks.Count < TotalChunks)
        {
            ResolveLoadedChunks();
            if (prevNumber < Chunks.Count && Chunks.Count % 10 == 0)
            {
                Logger.Info($"Loaded {Chunks.Count} / {TotalChunks} chunks");
                prevNumber = Chunks.Count;
            }
        }
    }

    private void RunJobs()
    {
        while (!_cancellationTokenSource.IsCancellationRequested)
        {
            try
            {
                foreach (var jobType in _jobs.GetConsumingEnumerable(_cancellationTokenSource.Token))
                {
                    while (!_chunksToUpdateQueue.IsEmpty)
                    {
                        UpdateChunks();
                    }

                    if (_chunksToUpdateQueue.IsEmpty)
                        IsUpdating = false;

                    switch (jobType)
                    {
                        case JobType.Load:
                            LoadChunks();
                            break;
                        case JobType.Save:
                            SaveChunks();
                            break;
                        case JobType.Update:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }

    private void LoadChunks()
    {
        if (!_chunksToLoad.TryDequeue(out var position))
            return;

        var chunk = _savedChunks.Contains(position) ? _chunkHandler.LoadChunk(position) :
            _chunkFactory.GenerateChunk(position * Chunk.Size, false);

        _loadedChunks.Enqueue(chunk);
    }

    private void SaveChunks()
    {
        if (!_chunksToSaveQueue.TryDequeue(out var position))
            return;

        _chunksToSaveDictionary.TryRemove(position, out var voxels);
        _chunkHandler.SaveChunkData(position, new ChunkData { Voxels = voxels!, SphereId = 0 });
        _savedChunks.Add(position / Chunk.Size);
    }

    private void UpdateChunks()
    {
        if (!_chunksToUpdateQueue.TryDequeue(out var chunk))
        {
            IsUpdating = false;
            return;
        }

        chunk.Mesh.Vertices = _meshGenerator.GetMesh(chunk.Position, new ChunkData { SphereId = 0, Voxels = chunk.Voxels });
        _updatedChunks.Enqueue(chunk);
    }

    public void Update(Vector3 currentPosition)
    {
        Vector3i currentChunk = GetCurrentChunkId(currentPosition);

        DeleteChunks(currentChunk);
        EnqueueLoadingChunks(currentChunk);
        ResolveLoadedChunks();
        ResolveUpdatedChunks();
    }

    private void DeleteChunks(Vector3i currentChunk)
    {
        if (_chunksToLoad.Count + Chunks.Count <= 2 * TotalChunks)
            return;

        Chunks.RemoveAll(chunk =>
        {
            if (!(GetDistance(chunk.Position / Chunk.Size, currentChunk) > _renderDistance))
                return false;

            _existingChunks.Remove(chunk.Position / Chunk.Size);
            if (_chunksToSaveDictionary.ContainsKey(chunk.Position))
            {
                _chunksToSaveDictionary[chunk.Position] = chunk.Voxels;
            }
            else
            {
                _chunksToSaveDictionary.TryAdd(chunk.Position, chunk.Voxels);
                _chunksToSaveQueue.Enqueue(chunk.Position);
                _jobs.Add(JobType.Save);
            }
            chunk.Dispose(_simulationManager.Simulation, _simulationManager.BufferPool);

            return true;
        });
    }

    private void EnqueueLoadingChunks(Vector3i currentChunk)
    {
        for (int x = -_renderDistance; x <= _renderDistance; x++)
        {
            for (int y = -_renderDistance; y <= _renderDistance; y++)
            {
                for (int z = -_renderDistance; z <= _renderDistance; z++)
                {
                    var chunk = new Vector3i(currentChunk.X + x, currentChunk.Y + y, currentChunk.Z + z);
                    if (_existingChunks.Contains(chunk))
                        continue;

                    _existingChunks.Add(chunk);
                    _chunksToLoad.Enqueue(chunk);
                    _jobs.Add(JobType.Load);
                }
            }
        }
    }

    public void EnqueueUpdatingChunk(Chunk chunk)
    {
        if (_chunksToUpdateHashSet.Contains(chunk))
            return;

        IsUpdating = true;
        _chunksToUpdateHashSet.Add(chunk);
        _chunksToUpdateQueue.Enqueue(chunk);
        _jobs.Add(JobType.Update);
    }

    private void ResolveLoadedChunks()
    {
        while (_loadedChunks.TryDequeue(out var chunk))
        {
            chunk.Mesh.CreateVertexArrayObject();
            chunk.CreateCollisionSurface(_simulationManager.Simulation, _simulationManager.BufferPool);
            Chunks.Add(chunk);
        }
    }

    private void ResolveUpdatedChunks()
    {
        while (_updatedChunks.TryDequeue(out var chunk))
        {
            chunk.Mesh.Update();
            chunk.UpdateCollisionSurface(_simulationManager.Simulation, _simulationManager.BufferPool);
            _chunksToUpdateHashSet.Remove(chunk);
        }
    }

    private static Vector3i GetCurrentChunkId(Vector3 position)
    {
        return new Vector3i((int)MathF.Floor(position.X / Chunk.Size), (int)MathF.Floor(position.Y / Chunk.Size), (int)MathF.Floor(position.Z / Chunk.Size));
    }

    private static float GetDistance(Vector3i a, Vector3i b)
    {
        return Math.Max(Math.Max(Math.Abs(a.X - b.X), Math.Abs(a.Y - b.Y)), Math.Abs(a.Z - b.Z));
    }

    private void SaveAllChunks()
    {
        foreach (var chunk in Chunks)
        {
            _chunkHandler.SaveChunkData(chunk.Position, new ChunkData { Voxels = chunk.Voxels, SphereId = 0 });
        }

        while (_chunksToSaveQueue.TryDequeue(out var position))
        {
            _chunksToSaveDictionary.TryRemove(position, out var voxels);
            _chunkHandler.SaveChunkData(position, new ChunkData { Voxels = voxels!, SphereId = 0 });
        }
    }

    private void GetSavedChunks()
    {
        var savedChunks = _chunkHandler.GetSavedChunks();
        foreach (var position in savedChunks)
        {
            _savedChunks.Add(position);
        }
    }

    public void Dispose()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();

        SaveAllChunks();
    }
}