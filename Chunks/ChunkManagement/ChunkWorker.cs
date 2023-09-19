﻿using System.Collections.Concurrent;
using Chunks.MarchingCubes;
using Chunks.Voxels;
using Common.UserInput;
using NLog;
using OpenTK.Mathematics;
using Physics.Collisions;

namespace Chunks.ChunkManagement;

public class ChunkWorker : IInputSubscriber
{
    private enum JobType
    {
        Load,
        Save,
        Update
    }

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

    private const int RenderDistance = 1;

    private const int NumberOfThreads = 2;

    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    private ChunkHandler _chunkHandler;

    private static int CacheNumber => 2 * (2 * RenderDistance + 1) * (2 * RenderDistance + 1) * (2 * RenderDistance + 1);

    private CancellationTokenSource _cancellationTokenSource = new();

    public ChunkWorker(List<Chunk> chunks, SimulationManager<PoseIntegratorCallbacks> simulationManager, ChunkFactory chunkFactory)
    {
        _chunks = chunks;
        _simulationManager = simulationManager;
        _chunkFactory = chunkFactory;
        _chunkHandler = new ChunkHandler();
        foreach (var chunk in _chunks)
        {
            _existingChunks.Add(chunk.Position / Chunk.Size);
        }

        RegisterCallbacks();
    }

    private void Start()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        for (int i = 0; i < NumberOfThreads; i++)
        {
            Task.Run(RunJobs);
        }

        GetSavedChunks();
        EnqueueLoadingChunks(Vector3i.Zero);
        int totalChunks = (2 * RenderDistance + 1) * (2 * RenderDistance + 1) * (2 * RenderDistance + 1);
        int prevNumber = 0;
        while (_chunks.Count < totalChunks)
        {
            ResolveLoadedChunks();
            if (prevNumber < _chunks.Count && _chunks.Count % 10 == 0)
            {
                Logger.Info($"Loaded {_chunks.Count} / {totalChunks} chunks");
                prevNumber = _chunks.Count;
            }
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

        var chunk = _savedChunks.Contains(position) ? _chunkHandler.LoadChunk(position) :
            _chunkFactory.GenerateChunk(position * Chunk.Size, false);

        _loadedChunks.Enqueue(chunk);
    }

    private void SaveChunks()
    {
        if (!_chunksToSaveQueue.TryDequeue(out var position)) return;

        _chunksToSaveDictionary.TryRemove(position, out var voxels);
        _chunkHandler.SaveChunkData(voxels!, position);
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
            chunk.CreateCollisionSurface(_simulationManager.Simulation, _simulationManager.BufferPool);
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

    private void SaveAllChunks()
    {
        foreach (var chunk in _chunks)
        {
            _chunkHandler.SaveChunkData(chunk.Voxels, chunk.Position);
        }

        while (true)
        {
            if (!_chunksToSaveQueue.TryDequeue(out var position)) return;

            _chunksToSaveDictionary.TryRemove(position, out var voxels);
            _chunkHandler.SaveChunkData(voxels!, position);
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

    public void RegisterCallbacks()
    {
        var context = Context.Instance;

        context.RegisterStartCallback(Start);

        context.RegisterCloseCallback(() =>
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();

            _existingChunks.Clear();
            _chunksToLoad.Clear();
            _loadedChunks.Clear();
            _savedChunks.Clear();
            _chunksToUpdateQueue.Clear();
            _chunksToUpdateHashSet.Clear();
            _updatedChunks.Clear();

            SaveAllChunks();
            _chunks.Clear();
            _chunksToSaveQueue.Clear();
            _chunksToSaveDictionary.Clear();
        });
    }
}