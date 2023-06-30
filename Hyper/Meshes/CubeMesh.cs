using OpenTK.Mathematics;

namespace Hyper.Meshes
{
    public static class CubeMesh
    {
        /*public static Mesh Create(float size, Vector3 position)
        {
            var vertexes = CreateCubeVertices(size);
            var positions = CreateCubeIndices();

            return new Mesh(vertexes, positions, position);
        }*/

        public static Mesh Create(Vector3 position)
        {
            return new Mesh(_vertices, position);
        }

        // Method to create vertices for the cube based on size and position
        private static Vertex[] CreateCubeVertices(float size)
        {
            float halfSize = size / 2f;
            return new Vertex[]
            {
                // Front face
                new Vertex { Position = new Vector3(-halfSize, -halfSize, halfSize), TexCoords = new Vector2(0.0f, 0.0f) },
                new Vertex { Position = new Vector3(halfSize, -halfSize, halfSize), TexCoords = new Vector2(1.0f, 0.0f) },
                new Vertex { Position = new Vector3(halfSize, halfSize, halfSize), TexCoords = new Vector2(1.0f, 1.0f) },
                new Vertex { Position = new Vector3(-halfSize, halfSize, halfSize), TexCoords = new Vector2(0.0f, 1.0f) },
        
                // Back face
                new Vertex { Position = new Vector3(-halfSize, -halfSize, -halfSize), TexCoords = new Vector2(0.0f, 0.0f) },
                new Vertex { Position = new Vector3(halfSize, -halfSize, -halfSize), TexCoords = new Vector2(1.0f, 0.0f) },
                new Vertex { Position = new Vector3(halfSize, halfSize, -halfSize), TexCoords = new Vector2(1.0f, 1.0f) },
                new Vertex { Position = new Vector3(-halfSize, halfSize, -halfSize) , TexCoords = new Vector2(0.0f, 1.0f) },

                // Left face
                new Vertex { Position = new Vector3(-halfSize, -halfSize, -halfSize) , TexCoords = new Vector2(0.0f, 0.0f) },
                new Vertex { Position = new Vector3(-halfSize, -halfSize, halfSize) , TexCoords = new Vector2(1.0f, 0.0f) },
                new Vertex { Position = new Vector3(-halfSize, halfSize, halfSize) , TexCoords = new Vector2(1.0f, 1.0f) },
                new Vertex { Position = new Vector3(-halfSize, halfSize, -halfSize) , TexCoords = new Vector2(0.0f, 1.0f) },

                // Right face
                new Vertex { Position = new Vector3(halfSize, -halfSize, -halfSize) , TexCoords = new Vector2(0.0f, 0.0f) },
                new Vertex { Position = new Vector3(halfSize, -halfSize, halfSize) , TexCoords = new Vector2(1.0f, 0.0f) },
                new Vertex { Position = new Vector3(halfSize, halfSize, halfSize) , TexCoords = new Vector2(1.0f, 1.0f) },
                new Vertex { Position = new Vector3(halfSize, halfSize, -halfSize) , TexCoords = new Vector2(0.0f, 1.0f) },

                // Top face
                new Vertex { Position = new Vector3(-halfSize, halfSize, -halfSize) , TexCoords = new Vector2(0.0f, 0.0f) },
                new Vertex { Position = new Vector3(halfSize, halfSize, -halfSize) , TexCoords = new Vector2(1.0f, 0.0f) },
                new Vertex { Position = new Vector3(halfSize, halfSize, halfSize) , TexCoords = new Vector2(1.0f, 1.0f) },
                new Vertex { Position = new Vector3(-halfSize, halfSize, halfSize) , TexCoords = new Vector2(0.0f, 1.0f) },

                // Bottom face
                new Vertex { Position = new Vector3(-halfSize, -halfSize, -halfSize) , TexCoords = new Vector2(0.0f, 0.0f) },
                new Vertex { Position = new Vector3(halfSize, -halfSize, -halfSize) , TexCoords = new Vector2(1.0f, 0.0f) },
                new Vertex { Position = new Vector3(halfSize, -halfSize, halfSize) , TexCoords = new Vector2(1.0f, 1.0f) },
                new Vertex { Position = new Vector3(-halfSize, -halfSize, halfSize) , TexCoords = new Vector2(0.0f, 1.0f) },
            };
        }

        // Method to create indices for the cube
        private static int[] CreateCubeIndices()
        {
            return new int[]
            {
                // Front face
                0, 1, 2, 2, 3, 0,
                // Back face
                4, 5, 6, 6, 7, 4,
                // Left face
                8, 9, 10, 10, 11, 8,
                // Right face
                12, 13, 14, 14, 15, 12,
                // Top face
                16, 17, 18, 18, 19, 16,
                // Bottom face
                20, 21, 22, 22, 23, 20
            };
        }

        // vertices & normals
        private static readonly float[] _vertices =
        {
            -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
             0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
             0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
             0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
            -0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
            -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,

            -0.5f, -0.5f,  0.5f,  0.0f,  0.0f, 1.0f,
             0.5f, -0.5f,  0.5f,  0.0f,  0.0f, 1.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  0.0f, 1.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  0.0f, 1.0f,
            -0.5f,  0.5f,  0.5f,  0.0f,  0.0f, 1.0f,
            -0.5f, -0.5f,  0.5f,  0.0f,  0.0f, 1.0f,

            -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,
            -0.5f,  0.5f, -0.5f, -1.0f,  0.0f,  0.0f,
            -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,
            -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,
            -0.5f, -0.5f,  0.5f, -1.0f,  0.0f,  0.0f,
            -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,

             0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,
             0.5f,  0.5f, -0.5f,  1.0f,  0.0f,  0.0f,
             0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,
             0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,
             0.5f, -0.5f,  0.5f,  1.0f,  0.0f,  0.0f,
             0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,

            -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,
             0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,
             0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,
             0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,
            -0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,

            -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,
             0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,
            -0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,
            -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f
        };
    }
}
