using OpenTK.Mathematics;

namespace Chunks.Voxels;

public static class VoxelHelper
{
    public static Vector3 GetColor(VoxelType type)
    {
        return type switch
        {
            (VoxelType.Grass) => new Vector3(0.1f, 0.7f, 0.1f),
            (VoxelType.GrassRock) => new Vector3(0.5f, 0.5f, 0),
            (VoxelType.Rock) => new Vector3(0.7f, 0.1f, 0.1f),
            _ => Vector3.Zero
        };
    }
}