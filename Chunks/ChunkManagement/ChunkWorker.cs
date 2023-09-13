using System.Collections.Concurrent;
using OpenTK.Mathematics;
using Physics.Collisions;

namespace Chunks.ChunkManagement;

// The explanation of how this class works is in the GitHub repository wiki.
public class ChunkWorker
{
    private readonly BlockingCollection<Vector3i> _chunksToLoad = new(new ConcurrentQueue<Vector3i>());

    private readonly BlockingCollection<Vector3i> _chunksToSaveQueue = new(new ConcurrentQueue<Vector3i>());

    private readonly ConcurrentDictionary<Vector3i, int> _chunksToSaveDictionary = new();

    private readonly ConcurrentQueue<Chunk> _loadedChunks = new();

    private readonly HashSet<Vector3i> _existingChunks = new();

    private readonly HashSet<Vector3i> _savedChunks = new();

    private readonly List<Chunk> _chunks;

    private readonly SimulationManager<PoseIntegratorCallbacks> _simulationManager;

    private readonly ChunkFactory _chunkFactory;

    public const int RenderDistance = 2;

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

        Directory.CreateDirectory(ChunkFactory.SaveLocation);

        for (int i = 0; i < NumberOfThreads; i++)
        {
            Task.Run(() => LoadChunks(_cancellationTokenSource.Token));
        }

        Task.Run(() => SaveChunks(_cancellationTokenSource.Token));
    }

    private void LoadChunks(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            foreach (var position in _chunksToLoad.GetConsumingEnumerable(cancellationToken))
            {
                Chunk chunk;
                if (_savedChunks.Contains(position)) chunk = _chunkFactory.LoadChunk(position);
                else chunk = _chunkFactory.GenerateChunk(position * Chunk.Size, false);

                _loadedChunks.Enqueue(chunk);
            }
        }
    }

    private void SaveChunks(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            foreach (var chunkPosition in _chunksToSaveQueue.GetConsumingEnumerable(cancellationToken))
            {
                _chunksToSaveDictionary.TryRemove(chunkPosition, out int index);
                _chunkFactory.SaveChunkData(index, chunkPosition);
                _savedChunks.Add(chunkPosition / Chunk.Size);
                _chunkFactory.FreeVoxels.Add(index, cancellationToken);
            }
        }
    }

    public void Update(Vector3 currentPosition)
    {
        var currentChunk = GetCurrentChunkId(currentPosition);

        DeleteChunks(currentChunk);
        EnqueueLoadingChunks(currentChunk);
        ResolveLoadedChunks();
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
                _chunkFactory.FreeVoxels.Add(_chunksToSaveDictionary[chunk.Position], _cancellationTokenSource.Token);
                _chunksToSaveDictionary[chunk.Position] = chunk.VoxelsPoolIndex;
            }
            else
            {
                _chunksToSaveDictionary.TryAdd(chunk.Position, chunk.VoxelsPoolIndex);
                _chunksToSaveQueue.Add(chunk.Position, _cancellationTokenSource.Token);
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
                    _chunksToLoad.Add(chunk, _cancellationTokenSource.Token);
                }
            }
        }
    }

    private void ResolveLoadedChunks()
    {
        while (_loadedChunks.TryDequeue(out var chunk))
        {
            chunk.CreateVertexArrayObject();
            if (chunk.NumberOfVertices > 0) chunk.CreateCollisionSurface(_simulationManager.Simulation, _simulationManager.BufferPool);
            _chunks.Add(chunk);
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