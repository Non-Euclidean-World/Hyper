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
        private ConcurrentDictionary<Vector3i, Guid> _neededChunkPositions;
        private ConcurrentDictionary<Vector3i, Guid> _existingChunkPositions;
        private ConcurrentDictionary<Guid, Chunk> _existingChunks;
        private CancellationTokenSource _cancellationTokenSource;
        private ScalarFieldGenerator _scalarFieldGenerator;
        private ChunkFactory _chunkFactory;
        private SimulationManager<NarrowPhaseCallbacks, PoseIntegratorCallbacks> _simulationManager;

        public ChunkWorker(ConcurrentDictionary<Guid, Chunk> existingChunks, ScalarFieldGenerator scalarFieldGenerator, 
            SimulationManager<NarrowPhaseCallbacks, PoseIntegratorCallbacks> simulationManager)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _neededChunkPositions = new ConcurrentDictionary<Vector3i, Guid>();
            _existingChunkPositions = new ConcurrentDictionary<Vector3i, Guid>();
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

        private void ProcessItems()
        {
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                Parallel.ForEach(_neededChunkPositions, kvp =>
                {
                    if (!_existingChunkPositions.ContainsKey(kvp.Key))
                    {
                        _existingChunkPositions.TryAdd(kvp.Key, kvp.Value);
                        var chunk = _chunkFactory.GenerateChunk(kvp.Key);
                        chunk.CreateCollisionSurface(_simulationManager.Simulation, _simulationManager.BufferPool);
                        _existingChunks.TryAdd(kvp.Value, chunk);
                    }
                });
                Parallel.ForEach(_existingChunkPositions, kvp =>
                {
                    if (!_neededChunkPositions.ContainsKey(kvp.Key))
                    {
                        _existingChunkPositions.TryRemove(kvp.Key, out _);
                        _existingChunks.TryRemove(kvp.Value, out var chunk);
                        // multithreaded removal might need a mutex?
                        // chunk?.DisposeCollisionSurface(_simulationManager.Simulation, _simulationManager.BufferPool);
                    }
                });
                Thread.Sleep(100);
            }
        }

        public static void UpdateNeededChunksBasedOnPosition(ChunkWorker chunkWorker, Vector3 position)
        {
            const int chunkCreateRadius = 50;
            const int chunkRemoveRadius = 70;
            Func<float, int> snap = (x) => ((int)Math.Round(x) / (Chunk.Size - 3)) * (Chunk.Size - 3);
            for (int i = -chunkCreateRadius; i <= chunkCreateRadius; i += Chunk.Size - 3)
            {
                for (int j = -chunkCreateRadius; j <= chunkCreateRadius; j += Chunk.Size - 3)
                {
                    for (int k = -chunkCreateRadius; k <= chunkCreateRadius; k += Chunk.Size - 3)
                    {
                        var vec = new Vector3i(
                            snap(position.X + i),
                            snap(position.Y + j),
                            snap(position.Z + k));
                        chunkWorker._neededChunkPositions.TryAdd(vec, Guid.NewGuid());
                    }
                }
            }

            foreach (var pvk in chunkWorker._existingChunkPositions)
            {
                var distance = Math.Max(Math.Max(
                    Math.Abs(snap(position.X - pvk.Key.X)),
                    Math.Abs(snap(position.Y - pvk.Key.Y))),
                    Math.Abs(snap(position.Z - pvk.Key.Z)));

                if (distance > chunkRemoveRadius)
                {
                    if (chunkWorker._neededChunkPositions.TryRemove(pvk.Key, out var chunk))
                    {
                        // this is an issue i have not resolved yet, singlethreaded removal also creates issues/dataraces
                        // chunkWorker._existingChunks[chunk].DisposeCollisionSurface(chunkWorker._simulationManager.Simulation, chunkWorker._simulationManager.BufferPool);
                    }
                }
            }
        }
    }
}