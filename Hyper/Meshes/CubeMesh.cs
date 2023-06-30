using OpenTK.Mathematics;

namespace Hyper.Meshes
{
    public static class CubeMesh
    {
        public static Mesh Create(float size, Vector3 position)
        {
            var vertexes = CreateCubeVertices(size, position);
            var positions = CreateCubeIndices();

            return new Mesh(vertexes, positions, position);
        }

        // Method to create vertices for the cube based on size and position
        private static Vertex[] CreateCubeVertices(float size, Vector3 position)
        {
            float halfSize = size / 2f;
            return new Vertex[]
            {
                // Front face
                new Vertex { Position = new Vector3(-halfSize, -halfSize, halfSize) + position, TexCoords = new Vector2(0.0f, 0.0f) },
                new Vertex { Position = new Vector3(halfSize, -halfSize, halfSize) + position, TexCoords = new Vector2(1.0f, 0.0f) },
                new Vertex { Position = new Vector3(halfSize, halfSize, halfSize) + position, TexCoords = new Vector2(1.0f, 1.0f) },
                new Vertex { Position = new Vector3(-halfSize, halfSize, halfSize) + position, TexCoords = new Vector2(0.0f, 1.0f) },
        
                // Back face
                new Vertex { Position = new Vector3(-halfSize, -halfSize, -halfSize) + position, TexCoords = new Vector2(0.0f, 0.0f) },
                new Vertex { Position = new Vector3(halfSize, -halfSize, -halfSize) + position, TexCoords = new Vector2(1.0f, 0.0f) },
                new Vertex { Position = new Vector3(halfSize, halfSize, -halfSize) + position, TexCoords = new Vector2(1.0f, 1.0f) },
                new Vertex { Position = new Vector3(-halfSize, halfSize, -halfSize) + position, TexCoords = new Vector2(0.0f, 1.0f) },

                // Left face
                new Vertex { Position = new Vector3(-halfSize, -halfSize, -halfSize) + position, TexCoords = new Vector2(0.0f, 0.0f) },
                new Vertex { Position = new Vector3(-halfSize, -halfSize, halfSize) + position, TexCoords = new Vector2(1.0f, 0.0f) },
                new Vertex { Position = new Vector3(-halfSize, halfSize, halfSize) + position, TexCoords = new Vector2(1.0f, 1.0f) },
                new Vertex { Position = new Vector3(-halfSize, halfSize, -halfSize) + position, TexCoords = new Vector2(0.0f, 1.0f) },

                // Right face
                new Vertex { Position = new Vector3(halfSize, -halfSize, -halfSize) + position, TexCoords = new Vector2(0.0f, 0.0f) },
                new Vertex { Position = new Vector3(halfSize, -halfSize, halfSize) + position, TexCoords = new Vector2(1.0f, 0.0f) },
                new Vertex { Position = new Vector3(halfSize, halfSize, halfSize) + position, TexCoords = new Vector2(1.0f, 1.0f) },
                new Vertex { Position = new Vector3(halfSize, halfSize, -halfSize) + position, TexCoords = new Vector2(0.0f, 1.0f) },

                // Top face
                new Vertex { Position = new Vector3(-halfSize, halfSize, -halfSize) + position, TexCoords = new Vector2(0.0f, 0.0f) },
                new Vertex { Position = new Vector3(halfSize, halfSize, -halfSize) + position, TexCoords = new Vector2(1.0f, 0.0f) },
                new Vertex { Position = new Vector3(halfSize, halfSize, halfSize) + position, TexCoords = new Vector2(1.0f, 1.0f) },
                new Vertex { Position = new Vector3(-halfSize, halfSize, halfSize) + position, TexCoords = new Vector2(0.0f, 1.0f) },

                // Bottom face
                new Vertex { Position = new Vector3(-halfSize, -halfSize, -halfSize) + position, TexCoords = new Vector2(0.0f, 0.0f) },
                new Vertex { Position = new Vector3(halfSize, -halfSize, -halfSize) + position, TexCoords = new Vector2(1.0f, 0.0f) },
                new Vertex { Position = new Vector3(halfSize, -halfSize, halfSize) + position, TexCoords = new Vector2(1.0f, 1.0f) },
                new Vertex { Position = new Vector3(-halfSize, -halfSize, halfSize) + position, TexCoords = new Vector2(0.0f, 1.0f) },
            };
        }

        // Method to create indices for the cube
        private static int[] CreateCubeIndices()
        {
            return new int[]
            {
                // Front face
                0, 1, 2, 3,
                // Back face
                5, 4, 7, 6,
                // Left face
                8, 9, 10, 11,
                // Right face
                13, 12, 15, 14,
                // Top face
                16, 17, 18, 19,
                // Bottom face
                21, 20, 23, 22
            };
        }

    }
}
