using Hyper.Meshes;
using OpenTK.Audio.OpenAL.Extensions.Creative.EnumerateAll;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hyper.MarchingCubes
{
    internal class Generator
    {
        private PerlinNoise _perlin;
        private int _chunkSize;

        public Generator(int seed = 0, int chunkSize = 16)
        {
            _perlin = new PerlinNoise(seed);
            _chunkSize = chunkSize;
        }

        public List<Object3D> GenerateWrold()
        {
            var object3d = new Object3D();
            object3d.Meshes.Add(GenerateChunk(new Vector3(0, 0, 0)));

            return new List<Object3D> { object3d };
        }

        public Mesh GenerateChunk(Vector3 position)
        {
            var renderer = new Renderer(GenerateScalarField(_chunkSize));
            Triangle[] triangles = renderer.GetMesh();
            Vector3[] normals = GetNormals(triangles);
            float[] data = GetTriangleAndNormalData(triangles, normals);

            return new Mesh(data, position);
        }

        public float[,,] GenerateScalarField(int width, int height, int depth)
        {
            float[,,] scalarField = new float[width, height, depth];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int z = 0; z < depth; z++)
                    {
                        scalarField[x, y, z] = PerlinNoise3D(x, y, z);
                    }
                }
            }

            return scalarField;
        }

        public float[,,] GenerateScalarField(int size) => GenerateScalarField(size, size, size);

        private float PerlinNoise3D(float x, float y, float z)
        {
            float px = x * 0.1553f;
            float py = y * 0.1271f;
            float pz = z * 0.0916f;

            float noise = (_perlin.GetNoise(px, py, pz) + 1) / 2.0f;

            return noise;
        }

        public Vector3 GetNormal(Triangle triangle)
        {
            // Calculate two vectors from the three points
            Vector3 edge1 = triangle.B - triangle.A;
            Vector3 edge2 = triangle.C - triangle.A;

            // Use the cross product to get the normal
            Vector3 normal = Vector3.Cross(edge1, edge2);

            if (normal.Y < 0)
            {
                normal = -normal;
            }

            // Normalize the normal
            normal.Normalize();

            return normal;
        }

        public Vector3[] GetNormals(Triangle[] triangles)
        {
            Vector3[] normals = new Vector3[triangles.Length];

            for (int i = 0; i < triangles.Length; i++)
            {
                normals[i] = GetNormal(triangles[i]);
            }

            return normals;
        }

        public float[] GetTriangleAndNormalData(Triangle[] triangles, Vector3[] normals)
        {
            if (triangles.Length != normals.Length)
            {
                throw new ArgumentException("Triangles and normals arrays must be the same length");
            }

            // Each triangle contributes 9 float values (3 vertices * 3 dimensions) and 
            // each normal contributes 3 float values (1 normal * 3 dimensions) 
            float[] data = new float[triangles.Length * 18]; // increased from 12 to 18

            for (int i = 0; i < triangles.Length; i++)
            {
                // Vertex A
                data[i * 18] = triangles[i].A.X;
                data[i * 18 + 1] = triangles[i].A.Y;
                data[i * 18 + 2] = triangles[i].A.Z;
                data[i * 18 + 3] = normals[i].X;
                data[i * 18 + 4] = normals[i].Y;
                data[i * 18 + 5] = normals[i].Z;

                // Vertex B
                data[i * 18 + 6] = triangles[i].B.X;
                data[i * 18 + 7] = triangles[i].B.Y;
                data[i * 18 + 8] = triangles[i].B.Z;
                data[i * 18 + 9] = normals[i].X;
                data[i * 18 + 10] = normals[i].Y;
                data[i * 18 + 11] = normals[i].Z;

                // Vertex C
                data[i * 18 + 12] = triangles[i].C.X;
                data[i * 18 + 13] = triangles[i].C.Y;
                data[i * 18 + 14] = triangles[i].C.Z;
                data[i * 18 + 15] = normals[i].X;
                data[i * 18 + 16] = normals[i].Y;
                data[i * 18 + 17] = normals[i].Z;

                // Note: This function assumes the same normal for A, B and C.
                // If you have per-vertex normals, you would need to adjust this function accordingly.
            }

            return data;
        }
    }
}
