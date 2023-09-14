using System.Collections.Concurrent;
using Chunks.MarchingCubes;
using Chunks.Voxels;
using OpenTK.Mathematics;
using Physics.Collisions;

namespace Chunks.ChunkManagement;

// The explanation of how this class works is in the GitHub repository wiki.
public class ChunkWorker
{
    private readonly BlockingCollection<JobType> _jobs = new(new ConcurrentQueue<JobType>());

    private readonly List<Chunk> _chunks;

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

    private const int RenderDistance = 2;

    private const int NumberOfThreads = 1;

    private static int CacheNumber => 2 * (2 * RenderDistance + 1) * (2 * RenderDistance + 1) * (2 * RenderDistance + 1);

    private readonly CancellationTokenSource _cancellationTokenSource = new();

    public ChunkWorker(List<Chunk> chunks, SimulationManager<PoseIntegratorCallbacks> simulationManager, ChunkFactory chunkFactory)
    {
        _chunks = chunks;
        _simulationManager = simulationManager;
        _chunkFactory = chunkFactory;
        foreach (var chunk in _chunks)
        {
            _existingChunks.Add(chunk.Position / Chunk.Size);
        }

        Directory.CreateDirectory(ChunkHandler.SaveLocation);

        for (int i = 0; i < NumberOfThreads; i++)
        {
            Task.Run(RunJobs);
        }
    }

    private void RunJobs()
    {
        while (!_cancellationTokenSource.IsCancellationRequested)
        {
            foreach (var jobType in _jobs.GetConsumingEnumerable(_cancellationTokenSource.Token))
            {
                while (!_chunksToUpdateQueue.IsEmpty)
                {
                    UpdateChunks();
                }

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
    }

    private void LoadChunks()
    {
        if (!_chunksToLoad.TryDequeue(out var position)) return;

        var chunk = _savedChunks.Contains(position) ? ChunkHandler.LoadChunk(position) :
            _chunkFactory.GenerateChunk(position * Chunk.Size, false);

        _loadedChunks.Enqueue(chunk);
    }

    private void SaveChunks()
    {
        if (!_chunksToSaveQueue.TryDequeue(out var position)) return;

        _chunksToSaveDictionary.TryRemove(position, out var voxels);
        ChunkHandler.SaveChunkData(voxels!, position);
        _savedChunks.Add(position / Chunk.Size);
    }

    private void UpdateChunks()
    {
        if (!_chunksToUpdateQueue.TryDequeue(out var chunk)) return;

        var renderer = new MeshGenerator(chunk.Voxels);
        chunk.Mesh.Vertices = renderer.GetMesh();
        _updatedChunks.Enqueue(chunk);
    }

    public void Update(Vector3 currentPosition)
    {
        var currentChunk = GetCurrentChunkId(currentPosition);

        DeleteChunks(currentChunk);
        EnqueueLoadingChunks(currentChunk);
        ResolveLoadedChunks();
        ResolveUpdatedChunks();
    }

    private void DeleteChunks(Vector3i currentChunk)
    {
        if (_chunksToLoad.Count + _chunks.Count <= CacheNumber) return;

        _chunks.RemoveAll(chunk =>
        {
            if (!(GetDistance(chunk.Position / Chunk.Size, currentChunk) > RenderDistance)) return false;

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
        for (int x = -RenderDistance; x <= RenderDistance; x++)
        {
            for (int y = -RenderDistance; y <= RenderDistance; y++)
            {
                for (int z = -RenderDistance; z <= RenderDistance; z++)
                {
                    var chunk = new Vector3i(currentChunk.X + x, currentChunk.Y + y, currentChunk.Z + z);
                    if (_existingChunks.Contains(chunk)) continue;

                    _existingChunks.Add(chunk);
                    _chunksToLoad.Enqueue(chunk);
                    _jobs.Add(JobType.Load);
                }
            }
        }
    }

    public void EnqueueUpdatingChunk(Chunk chunk)
    {
        if (_chunksToUpdateHashSet.Contains(chunk)) return;

        _chunksToUpdateHashSet.Add(chunk);
        _chunksToUpdateQueue.Enqueue(chunk);
        _jobs.Add(JobType.Update);
    }

    private void ResolveLoadedChunks()
    {
        while (_loadedChunks.TryDequeue(out var chunk))
        {
            chunk.Mesh.CreateVertexArrayObject();
            if (chunk.Mesh.Vertices.Length > 0) chunk.CreateCollisionSurface(_simulationManager.Simulation, _simulationManager.BufferPool);
            _chunks.Add(chunk);
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
}