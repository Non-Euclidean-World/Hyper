using Chunks.Voxels;
using OpenTK.Mathematics;

namespace Chunks.MarchingCubes;

public class ScalarFieldGenerator
{
    private readonly int _seed;

    private int _octaves = 3;

    private float _initialFreq = 0.25f;

    private float _freqMul = 2f;

    private float _initialAmp = 16f;

    private float _ampMul = 0.5f;

    private readonly float _maxAmp;
    
    private float _verticalOffset = 0f;

    private float _middleLevel = 10;
    
    private float _topLevel = 18;

    public float AvgElevation { get; private set; } = 0f;

    public ScalarFieldGenerator(int seed)
    {
        _seed = seed;
        GetTerrainSettings(seed);
        _maxAmp = AvgElevation = GetMaxAmp();
    }

    /// <summary>
    /// Generates a 3d scalar field of voxels.
    /// </summary>
    /// <param name="width">The size in X dimension.</param>
    /// <param name="height">The size in Y dimension.</param>
    /// <param name="depth">The size in Z dimension.</param>
    /// <param name="position">The position in the world where the scalar filed will be placed.</param>
    /// <returns></returns>
    public Voxel[,,] Generate(int width, int height, int depth, Vector3i position)
    {
        var perlin = new PerlinNoise(_seed);
        const int extra = Chunk.Overlap + 1;
        var scalarField = new Voxel[width + extra, height + extra, depth + extra];

        for (int x = position.X; x < width + extra + position.X; x++)
        {
            for (int y = position.Y; y < height + extra + position.Y; y++)
            {
                for (int z = position.Z; z < depth + extra + position.Z; z++)
                {
                    float density = y;
                    const float offset = -0.5f;
                    int octaves = _octaves;
                    float freq = _initialFreq;
                    float amp = _initialAmp;
                    for (int i = 0; i < octaves; i++)
                    {
                        density += (perlin.GetNoise3D(x * freq, y * freq, z * freq) + offset) * amp;
                        freq *= _freqMul;
                        amp *= _ampMul;
                    }
                    float value = density - _maxAmp - _verticalOffset;

                    VoxelType type;
                    if (y < _middleLevel) type = VoxelType.Bottom;
                    else if (y < _topLevel) type = VoxelType.Middle;
                    else type = VoxelType.Top;

                    scalarField[x - position.X, y - position.Y, z - position.Z] = new Voxel(value, type);
                }
            }
        }

        return scalarField;
    }

    /// <summary>
    /// Generates a 3d scalar field of voxels. Each size is the same.
    /// </summary>
    /// <param name="size"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    public Voxel[,,] Generate(int size, Vector3i position) => Generate(size, size, size, position);

    private float GetMaxAmp()
    {
        float maxAmp = 0f;
        float amp = _initialAmp;
        for (int i = 0; i < _octaves; i++)
        {
            maxAmp += amp * 0.5f;
            amp *= _ampMul;
        }

        return maxAmp;
    }

    private void GetTerrainSettings(int seed)
    {
        var index = seed % 5;
        if (index == 0)
            GetVolcanic();
        else if (index == 1)
            GetDessert();
        else if (index == 2)
            GetForrest();
        else if (index == 3)
            GetToxic();
        else
            GetJungle();
    }
    
    private void GetVolcanic()
    {
        _octaves = 4;
        _initialFreq = 0.30f;
        _initialAmp = 20f;
        _freqMul = 2f;
        _ampMul = 0.5f;
        
        _verticalOffset = 0;
        _middleLevel = 15;
        _topLevel = 23;
    }
    
    private void GetDessert()
    {
        _octaves = 2;
        _initialFreq = 0.27f;
        _initialAmp = 17f;
        _freqMul = 2f;
        _ampMul = 0.5f;
        
        _verticalOffset = 0;
        _middleLevel = 10;
        _topLevel = 14;
    }
    
    private void GetForrest()
    {
        _octaves = 3;
        _initialFreq = 0.20f;
        _initialAmp = 18f;
        _freqMul = 2f;
        _ampMul = 0.5f;
        
        _verticalOffset = 0;
        _middleLevel = 10;
        _topLevel = 18;
    }

    private void GetJungle()
    {
        _octaves = 4;
        _initialFreq = 0.22f;
        _initialAmp = 20f;
        _freqMul = 2f;
        _ampMul = 0.5f;
        
        _verticalOffset = 0;
        _middleLevel = 10;
        _topLevel = 18;
    }

    private void GetToxic()
    {
        _octaves = 3;
        _initialFreq = 0.25f;
        _initialAmp = 16f;
        _freqMul = 2f;
        _ampMul = 0.5f;
        
        _verticalOffset = 0;
        _middleLevel = 10;
        _topLevel = 18;
    }
}

