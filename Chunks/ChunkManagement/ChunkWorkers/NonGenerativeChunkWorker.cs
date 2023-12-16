using System.Collections.Concurrent;
using Chunks.MarchingCubes.MeshGenerators;
using OpenTK.Mathematics;
using Physics.Collisions;

namespace Chunks.ChunkManagement.ChunkWorkers;

public class NonGenerativeChunkWorker : IChunkWorker
{
    public List<Chunk> Chunks { get; }

    public bool IsProcessingBatch { get; set; }

    private readonly BlockingCollection<ModificationArgs> _modificationsToPerform = new(new ConcurrentQueue<ModificationArgs>());

    private readonly ConcurrentQueue<(Chunk, int)> _updatedChunks = new();

    private readonly ConcurrentQueue<Chunk> _loadedChunks = new();

    private readonly SimulationManager<PoseIntegratorCallbacks> _simulationManager;

    private readonly ChunkHandler _chunkHandler;

    private const int NumberOfThreads = 1;

    private readonly CancellationTokenSource _cancellationTokenSource = new();

    private readonly SphericalChunkFactory _chunkFactory;

    private readonly SphericalMeshGenerator _meshGenerator;

    public NonGenerativeChunkWorker(List<Chunk> chunks, SimulationManager<PoseIntegratorCallbacks> simulationManager, SphericalChunkFactory chunkFactory, ChunkHandler chunkHandler, SphericalMeshGenerator meshGenerator)
    {
        _simulationManager = simulationManager;
        _chunkFactory = chunkFactory;
        _chunkHandler = chunkHandler;
        _meshGenerator = meshGenerator;
        Chunks = chunks;

        Start();
    }

    private void Start()
    {
        for (int i = 0; i < NumberOfThreads; i++)
        {
            Task.Run(RunJob);
        }

        Chunks.Clear();
        var initialChunks = new List<Chunk>();
        initialChunks.AddRange(_chunkHandler.LoadAllSavedChunks(spherical: true));
        if (initialChunks.Count == 0)
            initialChunks.AddRange(_chunkFactory.CreateSpheres(chunksPerSide: 2, generateVao: false));
        foreach (var chunk in initialChunks)
            _loadedChunks.Enqueue(chunk);
        ResolveLoadedChunks();
        initialChunks.Clear();
    }

    private void RunJob()
    {
        try
        {
            foreach (var modification in _modificationsToPerform.GetConsumingEnumerable(_cancellationTokenSource.Token))
            {
                var modificationType = modification.ModificationType;
                var location = modification.Location;
                var time = modification.Time;
                var brushWeight = modification.BrushWeight;
                var radius = modification.Radius;
                var chunk = modification.Chunk;
                var batchSize = modification.BatchSize;

                if (modificationType == ModificationType.Mine)
                {
                    chunk.Mine(location, time, brushWeight, radius);
                }
                else if (modificationType == ModificationType.Build)
                {
                    chunk.Build(location, time, brushWeight, radius);
                }
                else
                    throw new NotImplementedException();

                var mesh = _meshGenerator.GetMesh(chunk.Position, new ChunkData { SphereId = chunk.Sphere, Voxels = chunk.Voxels });
                lock (chunk.UpdatingLock)
                    chunk.Mesh.Vertices = mesh;

                _updatedChunks.Enqueue((chunk, batchSize));
            }
        }
        catch (OperationCanceledException) { }
    }

    public void Update(Vector3 currentPosition)
    {
        ResolveLoadedChunks();
        ResolveUpdatedChunks();
    }

    private readonly List<Chunk> _currentBatch = new();

    private void ResolveUpdatedChunks()
    {
        while (_updatedChunks.TryDequeue(out var chunk))
        {
            _currentBatch.Add(chunk.Item1);

            if (_currentBatch.Count == chunk.Item2)
            {
                foreach (var c in _currentBatch)
                {
                    lock (c.UpdatingLock)
                    {
                        c.Mesh.Update();
                        c.UpdateCollisionSurface(_simulationManager.Simulation, _simulationManager.BufferPool);
                    }
                }
                IsProcessingBatch = false;
                _currentBatch.Clear();
            }
        }
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

    public void EnqueueModification(ModificationArgs modificationArgs)
    {
        _modificationsToPerform.Add(modificationArgs);
    }

    public void Dispose()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();

        foreach (var chunk in Chunks)
        {
            _chunkHandler.SaveChunkData(chunk.Position, new ChunkData { Voxels = chunk.Voxels, SphereId = chunk.Sphere }, spherical: true);
        }
    }
}
