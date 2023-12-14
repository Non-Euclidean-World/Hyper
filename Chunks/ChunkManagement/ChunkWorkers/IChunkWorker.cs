﻿using OpenTK.Mathematics;

namespace Chunks.ChunkManagement.ChunkWorkers;

public interface IChunkWorker : IDisposable
{
    public List<Chunk> Chunks { get; }

    public bool IsUpdating { get; }

    public void EnqueueModification(ModificationArgs modificationArgs);

    void Update(Vector3 currentPosition);
}
