using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
                foreach (var kvp in _neededChunkPositions)
                {
                    if (!_existingChunkPositions.ContainsKey(kvp.Key))
                    {
                        Console.WriteLine("mk" + kvp.Key.ToString());
                        _existingChunkPositions.TryAdd(kvp.Key, kvp.Value);
                        var chunk = _chunkFactory.GenerateChunk(kvp.Key);
                        chunk.CreateCollisionSurface(_simulationManager.Simulation, _simulationManager.BufferPool);
                        _existingChunks.TryAdd(kvp.Value, chunk);
                    }
                }

                foreach (var kvp in _existingChunkPositions)
                {
                    if (!_neededChunkPositions.ContainsKey(kvp.Key))
                    {
                        Console.WriteLine("rm" + kvp.Key.ToString());
                        _existingChunkPositions.TryRemove(kvp.Key, out _);
                        _existingChunks.TryRemove(kvp.Value, out var chunk);
                        chunk?.DisposeCollisionSurface(_simulationManager.Simulation, _simulationManager.BufferPool);
                    }
                }

                Thread.Sleep(100);
            }

            Console.WriteLine("Item processing completed.");
        }

        public void UpdateNeededChunksBasedOnPosition(Vector3 position)
        {
            Func<float, int, int> clampAndAddOffset = (x, offset) => ((int)Math.Round(x) / (Chunk.Size - 3) + offset) * (Chunk.Size - 3);
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    for (int k = -1; k < 2; k++)
                    {
                        var vec = new Vector3i(clampAndAddOffset(position.X, i),
                            clampAndAddOffset(position.Y, j),
                            clampAndAddOffset(position.Z, k));
                        _neededChunkPositions.TryAdd(vec, Guid.NewGuid());

                    }
                }
            }

            foreach (var pvk in _existingChunkPositions)
            {
                var distance = Math.Pow(pvk.Key.X - position.X, 2) + Math.Pow(pvk.Key.Y - position.Y, 2) +
                    Math.Pow(pvk.Key.Z - position.Z, 2);
                if (Math.Sqrt(distance) > Chunk.Size * 3)
                {
                    _neededChunkPositions.TryRemove(pvk.Key, out _);
                }
            }
        }
    }
}