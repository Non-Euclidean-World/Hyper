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

        public float AvgElevation { get; private set; } = 0f;

        public Generator(int seed, int octaves = 3, float initialFreq = 0.25f, float freqMul = 2f, float initialAmp = 16f, float ampMul = 0.5f)
        {
            _seed = seed;
            _octaves = octaves;
            _initialFreq = initialFreq;
            _freqMul = freqMul;
            _initialAmp = initialAmp;
            _ampMul = ampMul;

            _maxAmp = AvgElevation = GetMaxAmp();
        }

        public Chunk GenerateChunk(Vector3i position)
        {
            var voxels = GenerateScalarField(Chunk.Size, position);
            var renderer = new Renderer(voxels);
            Vertex[] data = renderer.GetMesh();

            return new Chunk(data, position, voxels);
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
