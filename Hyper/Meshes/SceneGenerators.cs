using OpenTK.Mathematics;

namespace Hyper.Meshes
{
    internal static class SceneGenerators
    {
        public static Vector3[] GenerateCubePositions(int nInDim)
        {
            float div = 5.0f;

            Vector3[] positions = new Vector3[nInDim * nInDim * nInDim];
            for (int i = 0; i < nInDim; i++)
                for (int j = 0; j < nInDim; j++)
                    for (int k = 0; k < nInDim; k++)
                    {
                        float x = 2 * (i - nInDim / 2f) / div;
                        float y = 2 * (j - nInDim / 2f) / div;
                        float z = 2 * (k - nInDim / 2f) / div;
                        positions[i * nInDim * nInDim + j * nInDim + k] = new Vector3(x, y, z);
                    }

            return positions;
        }

        public static Vector3[] GenerateFlat(int n)
        {
            List<Vector3> positions = new List<Vector3>();
            Random random = new Random(0);
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    int maxHeight;
                    if (i % 10 == 0 && j % 10 == 0)
                        maxHeight = 10;
                    else
                        maxHeight = random.Next(1, 4);

                    for (int k = 0; k < maxHeight; k++)
                    {
                        float x = (i - n / 2f);
                        float y = k;
                        float z = (j - n / 2f);
                        positions.Add(new Vector3(x, y, z));
                    }
                }
            }

            return positions.ToArray();
        }

        public static Vector3[] GenerateColumn(int n)
        {
            List<Vector3> positions = new List<Vector3>();


            for (int k = 0; k < n; k++)
            {
                float y = k;
                positions.Add(new Vector3(0, y, 0));
            }


            return positions.ToArray();
        }

        public static Vector3[] CreateSphereOfCubes(int cubeCount)
        {
            Vector3[] cubes = new Vector3[cubeCount];
            Random random = new Random();

            for (int i = 0; i < cubeCount; i++)
            {
                // Create a new cube
                cubes[i] = new Vector3();

                // Generate random theta and phi angles
                float theta = (float)(random.NextDouble() * 2 * Math.PI);
                float phi = (float)(random.NextDouble() * Math.PI);

                // Convert spherical coordinates to Cartesian coordinates
                float x = (float)(Math.Sin(phi) * Math.Cos(theta));
                float y = (float)(Math.Sin(phi) * Math.Sin(theta));
                float z = (float)(Math.Cos(phi));

                // Set the cube's position
                cubes[i] = new Vector3(x, y, z) * 0.8f;
            }

            return cubes;
        }

        public static void CreateUVSphere(float radius, int numLongitudeLines, int numLatitudeLines, out Vector3[] vertices, out int[] indices)
        {
            List<Vector3> verticesList = new List<Vector3>();
            List<int> indicesList = new List<int>();

            // Generate vertices
            for (int lat = 0; lat <= numLatitudeLines; ++lat)
            {
                float theta = lat * MathF.PI / numLatitudeLines;
                float sinTheta = MathF.Sin(theta);
                float cosTheta = MathF.Cos(theta);

                for (int lon = 0; lon <= numLongitudeLines; ++lon)
                {
                    float phi = lon * 2 * MathF.PI / numLongitudeLines;
                    float sinPhi = MathF.Sin(phi);
                    float cosPhi = MathF.Cos(phi);

                    float x = cosPhi * sinTheta;
                    float y = cosTheta;
                    float z = sinPhi * sinTheta;

                    verticesList.Add(new Vector3(x * radius, y * radius, z * radius));
                }
            }

            // Generate indices
            for (int lat = 0; lat < numLatitudeLines; ++lat)
            {
                for (int lon = 0; lon < numLongitudeLines; ++lon)
                {
                    int first = (lat * (numLongitudeLines + 1)) + lon;
                    int second = first + numLongitudeLines + 1;

                    indicesList.Add(first);
                    indicesList.Add(second);
                    indicesList.Add(first + 1);

                    indicesList.Add(second);
                    indicesList.Add(second + 1);
                    indicesList.Add(first + 1);
                }
            }

            vertices = verticesList.ToArray();
            indices = indicesList.ToArray();
        }
    }
}
