using OpenTK.Mathematics;

namespace Chunks.ChunkManagement.ChunkWorkers;

public interface IChunkWorker : IDisposable
{
    bool IsOnUpdateQueue(Chunk chunk);

    void EnqueueUpdatingChunk(Chunk chunk);

    void Update(Vector3 currentPosition);
}
