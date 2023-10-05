using System.Collections.Concurrent;
using Chunks.MarchingCubes.MeshGenerators;
using OpenTK.Mathematics;
using Physics.Collisions;

namespace Chunks.ChunkManagement.ChunkWorkers;

public class NonGenerativeChunkWorker : IChunkWorker, IDisposable
{
    private readonly BlockingCollection<Chunk> _chunksToUpdate = new(new ConcurrentQueue<Chunk>());

    private readonly ConcurrentHashSet<Chunk> _chunksToUpdateHashSet = new();

    private readonly ConcurrentQueue<Chunk> _updatedChunks = new();

    private readonly ConcurrentQueue<Chunk> _loadedChunks = new();

    private readonly List<Chunk> _chunks;

    private readonly SimulationManager<PoseIntegratorCallbacks> _simulationManager;

    private readonly ChunkHandler _chunkHandler;

    private const int NumberOfThreads = 1;

    private CancellationTokenSource _cancellationTokenSource = new();

    private readonly SphericalChunkFactory _chunkFactory;

    private readonly SphericalMeshGenerator _meshGenerator;

    public NonGenerativeChunkWorker(List<Chunk> chunks, SimulationManager<PoseIntegratorCallbacks> simulationManager, SphericalChunkFactory chunkFactory, ChunkHandler chunkHandler, SphericalMeshGenerator meshGenerator)
    {
        _simulationManager = simulationManager;
        _chunkFactory = chunkFactory;
        _chunkHandler = chunkHandler;
        _meshGenerator = meshGenerator;
        _chunks = chunks;

        Start();
    }

    private void Start()
    {
        for (int i = 0; i < NumberOfThreads; i++)
        {
            Task.Run(RunJob);
        }
    }

    private void RunJob()
    {
        _chunks.Clear();
        _chunks.AddRange(_chunkHandler.LoadAllSavedChunks(spherical: true));
        if (_chunks.Count == 0)
            _chunks.AddRange(_chunkFactory.CreateSpheres(chunksPerSide: 2, generateVao: false));

        foreach (var chunk in _chunks)
            _loadedChunks.Enqueue(chunk);
        try
        {
            while (_chunksToUpdate.TryTake(out var chunk, Timeout.Infinite, _cancellationTokenSource.Token))
            {
                chunk.Mesh.Vertices = _meshGenerator.GetMesh(chunk.Position, new ChunkHandler.ChunkData { SphereId = chunk.Sphere, Voxels = chunk.Voxels });
                _updatedChunks.Enqueue(chunk);
            }
        }
        catch (OperationCanceledException)
        {
            return;
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

    private void ResolveLoadedChunks()
    {
        while (_loadedChunks.TryDequeue(out var chunk))
        {
            chunk.Mesh.CreateVertexArrayObject();
            chunk.CreateCollisionSurface(_simulationManager.Simulation, _simulationManager.BufferPool);
        }
    }

    public void Dispose()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();

        foreach (var chunk in _chunks)
        {
            _chunkHandler.SaveChunkData(chunk.Position, new ChunkHandler.ChunkData { Voxels = chunk.Voxels, SphereId = chunk.Sphere }, spherical: true);
        }
    }

    public void EnqueueUpdatingChunk(Chunk chunk)
    {
        if (_chunksToUpdateHashSet.Contains(chunk))
            return;

        _chunksToUpdateHashSet.Add(chunk);
        _chunksToUpdate.Add(chunk);
    }

    public bool IsOnUpdateQueue(Chunk chunk)
    {
        return _chunksToUpdateHashSet.Contains(chunk);
    }

    public void Update(Vector3 currentPosition)
    {
        ResolveLoadedChunks();
        ResolveUpdatedChunks();
    }
}
