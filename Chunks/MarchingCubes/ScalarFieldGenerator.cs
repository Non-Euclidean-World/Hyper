﻿using Chunks.Voxels;
using OpenTK.Mathematics;

namespace Chunks.MarchingCubes;

public class ScalarFieldGenerator
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
                    float value = density - _maxAmp;

                    VoxelType type;
                    if (y < 10) type = VoxelType.Bottom;
                    else if (y < 18) type = VoxelType.Middle;
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
}

