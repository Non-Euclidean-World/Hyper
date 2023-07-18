using Hyper.Meshes;
using OpenTK.Mathematics;

namespace Hyper.MarchingCubes
{
    internal class Generator
    {
        private readonly int _seed;

        private readonly int _octaves;

        private readonly float _initialFreq;

        private readonly float _freqMul;

        private readonly float _initialAmp;

        private readonly float _ampMul;

        private readonly float _maxAmp;

        internal float AvgElevation { get; private set; } = 0f;

        internal Generator(int seed, int octaves = 3, float initialFreq = 0.25f, float freqMul = 2f, float initialAmp = 16f, float ampMul = 0.5f)
        {
            _seed = seed;
            _octaves = octaves;
            _initialFreq = initialFreq;
            _freqMul = freqMul;
            _initialAmp = initialAmp;
            _ampMul = ampMul;

            _maxAmp = AvgElevation = GetMaxAmp();
        }

        internal Chunk GenerateChunk(Vector3i position)
        {
            var voxels = GenerateScalarField(Chunk.Size, position);
            var renderer = new Renderer(voxels);
            Triangle[] triangles = renderer.GetMesh();
            float[] data = GetTriangleAndNormalData(triangles);

            return new Chunk(data, position, voxels);
        }

        // This function is kinda useless. We could just write floats instead of Triangles but I think this is more readable. If performance is an issue this could be deleted.
        internal static float[] GetTriangleAndNormalData(Triangle[] triangles)
        {
            float[] data = new float[triangles.Length * 18];

            for (int i = 0; i < triangles.Length; i++)
            {
                // Vertex A
                data[i * 18] = triangles[i].A.X;
                data[i * 18 + 1] = triangles[i].A.Y;
                data[i * 18 + 2] = triangles[i].A.Z;
                data[i * 18 + 3] = triangles[i].Na.X;
                data[i * 18 + 4] = triangles[i].Na.Y;
                data[i * 18 + 5] = triangles[i].Na.Z;

                // Vertex B
                data[i * 18 + 6] = triangles[i].B.X;
                data[i * 18 + 7] = triangles[i].B.Y;
                data[i * 18 + 8] = triangles[i].B.Z;
                data[i * 18 + 9] = triangles[i].Nb.X;
                data[i * 18 + 10] = triangles[i].Nb.Y;
                data[i * 18 + 11] = triangles[i].Nb.Z;

                // Vertex C
                data[i * 18 + 12] = triangles[i].C.X;
                data[i * 18 + 13] = triangles[i].C.Y;
                data[i * 18 + 14] = triangles[i].C.Z;
                data[i * 18 + 15] = triangles[i].Nc.X;
                data[i * 18 + 16] = triangles[i].Nc.Y;
                data[i * 18 + 17] = triangles[i].Nc.Z;
            }

            return data;
        }

        private float[,,] GenerateScalarField(int width, int height, int depth, Vector3i position)
        {
            var perlin = new PerlinNoise(_seed);
            float[,,] scalarField = new float[width, height, depth];

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
                        scalarField[x - position.X, y - position.Y, z - position.Z] = density - _maxAmp;
                    }
                }
            }

            return scalarField;
        }

        private float[,,] GenerateScalarField(int size, Vector3i position) => GenerateScalarField(size, size, size, position);

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
}
