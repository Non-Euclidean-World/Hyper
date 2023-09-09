using System.Collections.Concurrent;
using Chunks.MarchingCubes;
using OpenTK.Mathematics;
using Physics.Collisions;
using Physics.Collisions.Bepu;

namespace Chunks;

public class ChunkWorker
{
    private readonly ConcurrentQueue<Vector3i> _chunksToLoad = new();
    
    private readonly ConcurrentQueue<Chunk> _loadedChunks = new();
    
    private readonly ConcurrentDictionary<Vector3i, byte> _existingChunks = new();

    private readonly HashSet<Vector3i> _savedChunks = new();
    
    private readonly List<Chunk> _chunks;
    
    private readonly SimulationManager<NarrowPhaseCallbacks, PoseIntegratorCallbacks> _simulationManager;

    private const int RenderDistance = 3;

    private readonly ChunkFactory _chunkFactory;
    
    public ChunkWorker(List<Chunk> chunks, SimulationManager<NarrowPhaseCallbacks, PoseIntegratorCallbacks> simulationManager)
    {
        _chunks = chunks;
        _simulationManager = simulationManager;
        
        var scalarFieldGenerator = new ScalarFieldGenerator(1);
        _chunkFactory = new ChunkFactory(scalarFieldGenerator);
        foreach (var chunk in _chunks)
        {
            _existingChunks.TryAdd(chunk.Position / Chunk.Size, 0);
        }

        Directory.CreateDirectory(ChunkHandler.SaveLocation);
        var thread = new Thread(LoadChunks);
        thread.Start();
    }
    
    private void LoadChunks()
    {
        // TODO need to exit this thread when app closes.
        while (true)
        {
            if (_chunksToLoad.TryDequeue(out var position))
            {
                Chunk chunk;
                if (_savedChunks.Contains(position)) chunk = ChunkHandler.LoadChunk(position);
                else chunk = _chunkFactory.GenerateChunkWithoutVao(position * Chunk.Size);
                
                _loadedChunks.Enqueue(chunk);
            }
            else
            {
                Thread.Sleep(100);
            }
        }
    }

    public void Update(Vector3 currentPosition)
    {
        var currentChunk = GetCurrentChunk(currentPosition);
        
        DeleteChunks(currentChunk);
        EnqueueLoadingChunks(currentChunk);
        ResolveLoadedChunks();
    }

    private void DeleteChunks(Vector3i currentChunk)
    {
        var toRemove = _chunks.Where(x =>
                GetDistance(x.Position / Chunk.Size, currentChunk) > RenderDistance)
            .Select(x => x.Position / Chunk.Size);
        foreach (var position in toRemove)
        {
            _existingChunks.TryRemove(position, out _);
            Console.WriteLine($"Removed chunk at {position * Chunk.Size}");
        }
        _chunks.RemoveAll(chunk =>
        {
            if (!(GetDistance(chunk.Position / Chunk.Size, currentChunk) > RenderDistance)) return false;
            
            ChunkHandler.SaveChunk(chunk);
            _savedChunks.Add(chunk.Position / Chunk.Size);
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
                    if (_existingChunks.ContainsKey(chunk)) continue;

                    _existingChunks.TryAdd(chunk, 0);
                    _chunksToLoad.Enqueue(chunk);
                }
            }
        }
    }

    private void ResolveLoadedChunks()
    {
        while (_loadedChunks.TryDequeue(out var chunk))
        {
            Console.WriteLine($"Created chunk at {chunk.Position}");
            chunk.CreateVertexArrayObject();
            if (chunk.NumberOfVertices > 0) chunk.CreateCollisionSurface(_simulationManager.Simulation, _simulationManager.BufferPool);
            _chunks.Add(chunk);
        }
    }

    private Vector3i GetCurrentChunk(Vector3 position)
    {
        return new Vector3i((int)MathF.Floor(position.X / Chunk.Size), (int)MathF.Floor(position.Y / Chunk.Size), (int)MathF.Floor(position.Z / Chunk.Size));
    }

    private float GetDistance(Vector3i a, Vector3i b)
    {
        return Math.Max(Math.Max(Math.Abs(a.X - b.X), Math.Abs(a.Y - b.Y)), Math.Abs(a.Z - b.Z));
    }
}