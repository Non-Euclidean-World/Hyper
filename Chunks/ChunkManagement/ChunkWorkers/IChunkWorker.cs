using OpenTK.Mathematics;

namespace Chunks.ChunkManagement.ChunkWorkers;

public interface IChunkWorker : IDisposable
{
    public List<Chunk> Chunks { get; }

    bool IsOnUpdateQueue(Chunk chunk);

    void EnqueueUpdatingChunk(Chunk chunk);

    void Update(Vector3 currentPosition);
}
