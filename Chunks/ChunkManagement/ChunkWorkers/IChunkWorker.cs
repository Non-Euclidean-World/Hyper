using OpenTK.Mathematics;

namespace Chunks.ChunkManagement.ChunkWorkers;

public interface IChunkWorker : IDisposable
{
    public List<Chunk> Chunks { get; }
    
    public bool IsUpdating { get; }

    void EnqueueUpdatingChunk(Chunk chunk);

    void Update(Vector3 currentPosition);
}
