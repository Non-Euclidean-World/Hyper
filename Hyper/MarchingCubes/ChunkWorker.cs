using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Threading;
using Hyper.Collisions;
using Hyper.MarchingCubes;
using Hyper.Meshes;
using OpenTK.Mathematics;
using Hyper.Collisions.Bepu;

namespace Hyper
{
    internal class ChunkWorker
    {
        private Vector3 _position; 
        private const int MaxThreads = 5;
        private readonly ConcurrentDictionary<Vector3i, Guid> _neededChunkPositions;
        private readonly ConcurrentDictionary<Vector3i, Guid> _existingChunkPositions;
        private readonly ConcurrentDictionary<Guid, Chunk> _existingChunks;
        private readonly Semaphore _semaphore;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly ScalarFieldGenerator _scalarFieldGenerator;
        private readonly ChunkFactory _chunkFactory;
        private readonly SimulationManager<NarrowPhaseCallbacks, PoseIntegratorCallbacks> _simulationManager;
        
        public ChunkWorker(ConcurrentDictionary<Guid, Chunk> existingChunks, ScalarFieldGenerator scalarFieldGenerator, 
            SimulationManager<NarrowPhaseCallbacks, PoseIntegratorCallbacks> simulationManager)
        {
            _position = new(0, 0, 0);
            _cancellationTokenSource = new CancellationTokenSource();
            _neededChunkPositions = new ConcurrentDictionary<Vector3i, Guid>();
            _neededChunkPositions.TryAdd(new Vector3i(0, 0, 0), Guid.NewGuid()); // initial chunk...
            _existingChunkPositions = new ConcurrentDictionary<Vector3i, Guid>();
            _semaphore = new Semaphore(MaxThreads, MaxThreads);
            _existingChunks = existingChunks;
            _scalarFieldGenerator = scalarFieldGenerator;
            _chunkFactory = new ChunkFactory(_scalarFieldGenerator);
            _simulationManager = simulationManager;
        }
        public void StartProcessing()
        {
            var processingThread = new Thread(ProcessItems);
            processingThread.IsBackground = true;
            processingThread.Start();
        }

        public void StopProcessing()
        {
            _cancellationTokenSource.Cancel();
        }

        private void InsertChunk(KeyValuePair<Vector3i, Guid> kvp, CancellationToken token)
        {
            _semaphore.WaitOne();
            try
            {
                token.ThrowIfCancellationRequested();
                var chunk = _chunkFactory.GenerateChunk(kvp.Key);
                chunk.CreateCollisionSurface(_simulationManager.Simulation, _simulationManager.BufferPool);
                _existingChunks.TryAdd(kvp.Value, chunk);
            }
            catch (OperationCanceledException)
            {
                _existingChunkPositions.TryRemove(kvp.Key, out _);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private void RemoveChunk(KeyValuePair<Vector3i, Guid> kvp, CancellationToken token)
        {
            _semaphore.WaitOne();
            try
            {
                _existingChunks.TryRemove(kvp.Value, out var chunk);
                // multithreaded removal might need a mutex?
                // chunk?.DisposeCollisionSurface(_simulationManager.Simulation, _simulationManager.BufferPool);
            }
            catch (OperationCanceledException)
            {
                _existingChunkPositions.TryAdd(kvp.Key, kvp.Value);
            }
            finally
            {
                _semaphore.Release();
            }
        }
        private void ProcessItems()
        {
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                var threads = new List<Thread>();
                foreach (var kvp in _existingChunkPositions)
                {
                    if (!_neededChunkPositions.ContainsKey(kvp.Key))
                    {
                        _existingChunkPositions.TryRemove(kvp.Key, out _);
                        var thread = new Thread(() => RemoveChunk(kvp, _cancellationTokenSource.Token));
                        threads.Add(thread);
                        thread.Start();
                    }
                }
                foreach (var kvp in _neededChunkPositions)
                {
                    if (!_existingChunkPositions.ContainsKey(kvp.Key))
                    {
                        _existingChunkPositions.TryAdd(kvp.Key, kvp.Value);
                        var thread = new Thread(() => InsertChunk(kvp, _cancellationTokenSource.Token));
                        threads.Add(thread);
                        thread.Start();
                    }
                }
                foreach (var thread in threads)
                {
                    thread.Join();
                }
                UpdateNeededChunksBasedOnPosition();
            }
        }

        public static void UpdateChunkWorkerPosition(ChunkWorker chunkWorker, Vector3 position)
        {
            chunkWorker._position = new(position);
        }

        private void UpdateNeededChunksBasedOnPosition()
        {
            const int chunkCreateRadius = 150;
            const int chunkRemoveRadius = 170;
            Func<float, int> snap = (x) => ((int)Math.Round(x) / (Chunk.Size - 3)) * (Chunk.Size - 3);
            for (int i = -chunkCreateRadius; i <= chunkCreateRadius; i += Chunk.Size - 3)
            {
                for (int j = -chunkCreateRadius; j <= chunkCreateRadius; j += Chunk.Size - 3)
                {
                    for (int k = -chunkCreateRadius; k <= chunkCreateRadius; k += Chunk.Size - 3)
                    {
                        var vec = new Vector3i(
                            snap(_position.X + i),
                            snap(_position.Y + j),
                            snap(_position.Z + k));
                        _neededChunkPositions.TryAdd(vec, Guid.NewGuid());
                    }
                }
            }

            foreach (var pvk in _existingChunkPositions)
            {
                var distance = Math.Max(Math.Max(
                    Math.Abs(snap(_position.X - pvk.Key.X)),
                    Math.Abs(snap(_position.Y - pvk.Key.Y))),
                    Math.Abs(snap(_position.Z - pvk.Key.Z)));

                if (distance > chunkRemoveRadius)
                {
                    if (_neededChunkPositions.TryRemove(pvk.Key, out var chunk))
                    {
                        // this is an issue i have not resolved yet, singlethreaded removal also creates issues/dataraces
                        // chunkWorker._existingChunks[chunk].DisposeCollisionSurface(chunkWorker._simulationManager.Simulation, chunkWorker._simulationManager.BufferPool);
                    }
                }
            }
        }
    }
}