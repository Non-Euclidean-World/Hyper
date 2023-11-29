using OpenTK.Mathematics;

namespace Chunks.Voxels;

public static class VoxelHelper
{
    public static Vector3 GetColor(VoxelType type)
    {
        return type switch
        {
            (VoxelType.Grass) => new Vector3(0.57f, 0.63f, 0.22f),
            (VoxelType.GrassRock) => new Vector3(0.48f, 0.31f, 0.08f),
            (VoxelType.Rock) => new Vector3(0.33f, 0.33f, 0.28f),
            _ => Vector3.Zero
        };
    }
}