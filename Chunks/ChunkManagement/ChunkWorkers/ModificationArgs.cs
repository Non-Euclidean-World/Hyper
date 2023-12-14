using OpenTK.Mathematics;

namespace Chunks.ChunkManagement.ChunkWorkers;

public enum ModificationType
{
    Mine,
    Build
}

public struct ModificationArgs
{
    public ModificationType ModificationType;
    public Vector3 Location;
    public float Time;
    public float BrushWeight;
    public int Radius;
    public Chunk Chunk;
    public uint BatchNumber;
}
