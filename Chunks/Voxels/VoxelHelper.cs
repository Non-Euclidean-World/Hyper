using System.Runtime.CompilerServices;
using OpenTK.Mathematics;

namespace Chunks.Voxels;

public class VoxelHelper
{
    private readonly Vector3 _topColor;
    
    private readonly Vector3 _middleRockColor;
    
    private readonly Vector3 _bottomColor;
    
    public VoxelHelper(int seed)
    {
        var index = seed % 5;
        if (index == 0) // Volcanic
        {
            _topColor = new Vector3(1.0f, 0.3f, 0.0f);
            _middleRockColor = new Vector3(0.3f, 0.3f, 0.3f);
            _bottomColor = new Vector3(0.4f, 0.35f, 0.35f);
        }
        else if (index == 1) // Dessert
        {
            _topColor = new Vector3(0.9f, 0.85f, 0.7f);
            _middleRockColor = new Vector3(0.7f, 0.6f, 0.5f);
            _bottomColor = new Vector3(0.55f, 0.45f, 0.35f);
        }
        else if (index == 2) // Forrest
        {
            _topColor = new Vector3(0.2f, 0.5f, 0.2f);
            _middleRockColor = new Vector3(0.4f, 0.27f, 0.13f);
            _bottomColor = new Vector3(0.3f, 0.15f, 0.08f);
        }
        else if (index == 3) // Toxic
        {
            _topColor = new Vector3(0.6f, 0.7f, 0.1f);
            _middleRockColor = new Vector3(0.5f, 0.55f, 0.2f);
            _bottomColor = new Vector3(0.4f, 0.45f, 0.25f);
        }
        else // Jungle
        {
            _topColor = new Vector3(0.1f, 0.5f, 0.1f);
            _middleRockColor = new Vector3(0.25f, 0.35f, 0.15f);
            _bottomColor = new Vector3(0.3f, 0.25f, 0.2f);
        }
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector3 GetColor(VoxelType type)
    {
        return type switch
        {
            (VoxelType.Bottom) => _bottomColor,
            (VoxelType.Middle) => _middleRockColor,
            (VoxelType.Top) => _topColor,
            _ => Vector3.Zero
        };
    }
}