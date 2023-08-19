using Hyper.MarchingCubes.Voxels;
using Hyper.Meshes;
using OpenTK.Mathematics;

namespace Hyper.MarchingCubes;

internal class ScalarFieldGenerator
{
    private readonly int _seed;

    private readonly int _octaves;

    private readonly float _initialFreq;

    private readonly float _freqMul;

    private readonly float _initialAmp;

    private readonly float _ampMul;

    private readonly float _maxAmp;

    public float AvgElevation { get; private set; } = 0f;

    public ScalarFieldGenerator(int seed, int octaves = 3, float initialFreq = 0.25f, float freqMul = 2f, float initialAmp = 16f, float ampMul = 0.5f)
    {
        _seed = seed;
        _octaves = octaves;
        _initialFreq = initialFreq;
        _freqMul = freqMul;
        _initialAmp = initialAmp;
        _ampMul = ampMul;

        _maxAmp = AvgElevation = GetMaxAmp();
    }

    /// <summary>
    /// Generates a 3d scalar field of voxels.
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="depth"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    public Voxel[,,] Generate(int width, int height, int depth, Vector3i position)
    {
        var perlin = new PerlinNoise(_seed);
        Voxel[,,] scalarField = new Voxel[width, height, depth];

        for (int x = position.X; x < width + position.X; x++)
        {
            for (int y = position.Y; y < height + position.Y; y++)
            {
                for (int z = position.Z; z < depth + position.Z; z++)
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
                    float value = density - _maxAmp;

                    VoxelType type;
                    if (y < Chunk.Size / 2.5) type = VoxelType.Rock;
                    else if (y < Chunk.Size / 2) type = VoxelType.GrassRock;
                    else type = VoxelType.Grass;

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
}

