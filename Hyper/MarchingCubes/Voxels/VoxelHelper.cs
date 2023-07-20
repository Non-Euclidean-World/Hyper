using OpenTK.Mathematics;

namespace Hyper.MarchingCubes.Voxels;

public static class VoxelHelper
{
    public static Vector3 GetColor(VoxelType type)
    {
        return type switch
        {
            (VoxelType.Grass) => new Vector3(0, 1, 0),
            (VoxelType.GrassRock) => new Vector3(0.5f, 0.5f, 0),
            (VoxelType.Rock) => new Vector3(1, 0, 0),
            _ => Vector3.Zero
        };
    }
    
    
}