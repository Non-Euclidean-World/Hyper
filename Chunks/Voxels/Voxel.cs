namespace Chunks.Voxels;

public struct Voxel
{
    public float Value;

    public VoxelType Type;

    public Voxel(float value, VoxelType type)
    {
        Value = value;
        Type = type;
    }
}