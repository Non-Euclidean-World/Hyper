using Hyper.Meshes;
using OpenTK.Mathematics;

namespace Hyper.MarchingCubes
{
    internal class Generator
    {
        private PerlinNoise _perlin;
        private float _offsetY = 0f;

        public Generator(int seed = 0)
        {
            _perlin = new PerlinNoise(seed);
        }

        public Chunk GenerateChunk(Vector3i position)
        {
            var voxels = GenerateScalarField(Chunk.Size);
            var renderer = new Renderer(voxels);
            Triangle[] triangles = renderer.GetMesh();
            float[] data = GetTriangleAndNormalData(triangles);

            return new Chunk(data, position - Vector3i.UnitY * (int)_offsetY, voxels);
        }

        internal float[,,] GenerateScalarField(int width, int height, int depth)
        {
            float[,,] scalarField = new float[width, height, depth];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int z = 0; z < depth; z++)
                    {
                        float density = y;
                        float offset = -0.5f;
                        int octaves = 3;
                        float freq = 0.25f;
                        float amp = 16f;
                        float maxAmp = 0f;
                        for (int i = 0; i < octaves; i++)
                        {
                            density += (_perlin.GetNoise3D(x * freq, y * freq, z * freq) + offset) * amp;
                            freq *= 2;
                            amp /= 2;
                            maxAmp += amp * 0.5f;
                        }
                        scalarField[x, y, z] = (maxAmp - density + 10) / 20;
                        _offsetY = maxAmp;
                    }
                }
            }

            return scalarField;
        }

        internal float[,,] GenerateScalarField(int size) => GenerateScalarField(size, size, size);

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
    }
}
