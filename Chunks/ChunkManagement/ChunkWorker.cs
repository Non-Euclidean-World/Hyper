using System.Collections.Concurrent;
using Chunks.MarchingCubes;
using Chunks.Voxels;
using OpenTK.Mathematics;
using Physics.Collisions;
using Physics.Collisions.Bepu;

namespace Chunks.ChunkManagement;

public class ChunkWorker
{
    private readonly ConcurrentQueue<Vector3i> _chunksToLoad = new(); // Queue of chunks to load. Main thread writes. ManageChunks thread reads.

    private readonly ConcurrentQueue<(int VoxelIndex, Vector3i Position)> _chunksToSave = new(); // Queue of chunks to save. Main thread writes. ManageChunks thread reads.
    
    private readonly ConcurrentQueue<Chunk> _loadedChunks = new(); // Queue of loaded chunks. ManageChunks thread writes. Main thread reads.

    private readonly HashSet<Vector3i> _existingChunks = new(); // Set of chunks that are already created or are being created (are in _chunksToLoad).

    private readonly HashSet<Vector3i> _savedChunks = new(); // Set of chunks that were save to the disk.

    private readonly List<Chunk> _chunks;

    private readonly SimulationManager<NarrowPhaseCallbacks, PoseIntegratorCallbacks> _simulationManager;

    private readonly ChunkFactory _chunkFactory;

    public const int RenderDistance = 2;

    private const int NumberOfThreads = 1; // We can change this number to boost performance.

    public ChunkWorker(List<Chunk> chunks, SimulationManager<NarrowPhaseCallbacks, PoseIntegratorCallbacks> simulationManager, ChunkFactory chunkFactory)
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
            var thread = new Thread(ManageChunks)
            {
                IsBackground = true
            };
            thread.Start();
        }
    }

    private void ManageChunks()
    {
        while (true)
        {
            if (_chunksToLoad.TryDequeue(out var position))
            {
                Chunk chunk;
                if (_savedChunks.Contains(position)) chunk = _chunkFactory.LoadChunk(position);
                else 
                    chunk = _chunkFactory.GenerateChunk(position * Chunk.Size, false);

                _loadedChunks.Enqueue(chunk);
            }
            else if (_chunksToSave.TryDequeue(out var chunkData))
            {
                _chunkFactory.SaveChunkData(chunkData.VoxelIndex, chunkData.Position);
                _savedChunks.Add(chunkData.Position / Chunk.Size);
                _chunkFactory.FreeVoxels.Enqueue(chunkData.VoxelIndex);
            }
            else
            {
                Thread.Sleep(100);
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
        _chunks.RemoveAll(chunk =>
        {
            if (!(GetDistance(chunk.Position / Chunk.Size, currentChunk) > RenderDistance)) return false;

            _existingChunks.Remove(chunk.Position / Chunk.Size);
            _chunksToSave.Enqueue((chunk.VoxelsPoolIndex, chunk.Position));
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