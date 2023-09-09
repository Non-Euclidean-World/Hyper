using System.Collections.Concurrent;
using Chunks.MarchingCubes;
using Chunks.Voxels;
using NLog;
using OpenTK.Mathematics;
using Physics.Collisions;
using Physics.Collisions.Bepu;

namespace Chunks;

public class ChunkWorker
{
    private readonly ConcurrentQueue<Vector3i> _chunksToLoad = new();
    
    private readonly ConcurrentQueue<(Voxel[,,] Voxels, Vector3i Position)> _chunksToSave = new();
    
    private readonly ConcurrentQueue<Chunk> _loadedChunks = new();
    
    private readonly ConcurrentHashSet<Vector3i> _existingChunks = new();

    private readonly HashSet<Vector3i> _savedChunks = new();
    
    private readonly List<Chunk> _chunks;
    
    private readonly SimulationManager<NarrowPhaseCallbacks, PoseIntegratorCallbacks> _simulationManager;

    private const int RenderDistance = 3;

    private readonly ChunkFactory _chunkFactory;
    
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    
    public ChunkWorker(List<Chunk> chunks, SimulationManager<NarrowPhaseCallbacks, PoseIntegratorCallbacks> simulationManager, int seed)
    {
        _chunks = chunks;
        _simulationManager = simulationManager;
        
        var scalarFieldGenerator = new ScalarFieldGenerator(seed);
        _chunkFactory = new ChunkFactory(scalarFieldGenerator);
        foreach (var chunk in _chunks)
        {
            _existingChunks.TryAdd(chunk.Position / Chunk.Size);
        }

        Directory.CreateDirectory(ChunkHandler.SaveLocation);
        var thread = new Thread(LoadChunks)
        {
            IsBackground = true
        };
        thread.Start();
    }
    
    private void LoadChunks()
    {
        while (true)
        {
            if (_chunksToLoad.TryDequeue(out var position))
            {
                Chunk chunk;
                if (_savedChunks.Contains(position)) chunk = ChunkHandler.LoadChunk(position);
                else chunk = _chunkFactory.GenerateChunkWithoutVao(position * Chunk.Size);
                
                _loadedChunks.Enqueue(chunk);
            }
            else if (_chunksToSave.TryDequeue(out var chunkData))
            {
                ChunkHandler.SaveChunkData(chunkData.Voxels, chunkData.Position);
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
            _existingChunks.TryRemove(position);
            Logger.Info($"Removed chunk at {position * Chunk.Size}");
        }
        
        _chunks.RemoveAll(chunk =>
        {
            if (!(GetDistance(chunk.Position / Chunk.Size, currentChunk) > RenderDistance)) return false;
            
            _chunksToSave.Enqueue((chunk.Voxels, chunk.Position));
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
                    if (_existingChunks.Contains(chunk)) continue;

                    _existingChunks.TryAdd(chunk);
                    _chunksToLoad.Enqueue(chunk);
                }
            }
        }
    }

    private void ResolveLoadedChunks()
    {
        while (_loadedChunks.TryDequeue(out var chunk))
        {
            Logger.Info($"Created chunk at {chunk.Position}");
            chunk.CreateVertexArrayObject();
            if (chunk.NumberOfVertices > 0) chunk.CreateCollisionSurface(_simulationManager.Simulation, _simulationManager.BufferPool);
            _chunks.Add(chunk);
        }
    }

    private static Vector3i GetCurrentChunk(Vector3 position)
    {
        return new Vector3i((int)MathF.Floor(position.X / Chunk.Size), (int)MathF.Floor(position.Y / Chunk.Size), (int)MathF.Floor(position.Z / Chunk.Size));
    }

    private static float GetDistance(Vector3i a, Vector3i b)
    {
        return Math.Max(Math.Max(Math.Abs(a.X - b.X), Math.Abs(a.Y - b.Y)), Math.Abs(a.Z - b.Z));
    }
}