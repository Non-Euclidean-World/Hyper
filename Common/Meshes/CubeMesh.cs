using OpenTK.Mathematics;

namespace Common.Meshes;

public static class CubeMesh
{
    public static Mesh Create(Vector3 position)
    {
        return new Mesh(Vertices, position);
    }

    public static readonly Vertex[] Vertices =
    {
        new(-0.5f, -0.5f, -0.5f, 0.0f, 0.0f, -1.0f),
        new(0.5f, -0.5f, -0.5f, 0.0f, 0.0f, -1.0f),
        new(0.5f, 0.5f, -0.5f, 0.0f, 0.0f, -1.0f),
        new(0.5f, 0.5f, -0.5f, 0.0f, 0.0f, -1.0f),
        new(-0.5f, 0.5f, -0.5f, 0.0f, 0.0f, -1.0f),
        new(-0.5f, -0.5f, -0.5f, 0.0f, 0.0f, -1.0f),

        new(-0.5f, -0.5f, 0.5f, 0.0f, 0.0f, 1.0f),
        new(0.5f, -0.5f, 0.5f, 0.0f, 0.0f, 1.0f),
        new(0.5f, 0.5f, 0.5f, 0.0f, 0.0f, 1.0f),
        new(0.5f, 0.5f, 0.5f, 0.0f, 0.0f, 1.0f),
        new(-0.5f, 0.5f, 0.5f, 0.0f, 0.0f, 1.0f),
        new(-0.5f, -0.5f, 0.5f, 0.0f, 0.0f, 1.0f),

        new(-0.5f, 0.5f, 0.5f, -1.0f, 0.0f, 0.0f),
        new(-0.5f, 0.5f, -0.5f, -1.0f, 0.0f, 0.0f),
        new(-0.5f, -0.5f, -0.5f, -1.0f, 0.0f, 0.0f),
        new(-0.5f, -0.5f, -0.5f, -1.0f, 0.0f, 0.0f),
        new(-0.5f, -0.5f, 0.5f, -1.0f, 0.0f, 0.0f),
        new(-0.5f, 0.5f, 0.5f, -1.0f, 0.0f, 0.0f),

        new(0.5f, 0.5f, 0.5f, 1.0f, 0.0f, 0.0f),
        new(0.5f, 0.5f, -0.5f, 1.0f, 0.0f, 0.0f),
        new(0.5f, -0.5f, -0.5f, 1.0f, 0.0f, 0.0f),
        new(0.5f, -0.5f, -0.5f, 1.0f, 0.0f, 0.0f),
        new(0.5f, -0.5f, 0.5f, 1.0f, 0.0f, 0.0f),
        new(0.5f, 0.5f, 0.5f, 1.0f, 0.0f, 0.0f),

        new(-0.5f, -0.5f, -0.5f, 0.0f, -1.0f, 0.0f),
        new(0.5f, -0.5f, -0.5f, 0.0f, -1.0f, 0.0f),
        new(0.5f, -0.5f, 0.5f, 0.0f, -1.0f, 0.0f),
        new(0.5f, -0.5f, 0.5f, 0.0f, -1.0f, 0.0f),
        new(-0.5f, -0.5f, 0.5f, 0.0f, -1.0f, 0.0f),
        new(-0.5f, -0.5f, -0.5f, 0.0f, -1.0f, 0.0f),

        new(-0.5f, 0.5f, -0.5f, 0.0f, 1.0f, 0.0f),
        new(0.5f, 0.5f, -0.5f, 0.0f, 1.0f, 0.0f),
        new(0.5f, 0.5f, 0.5f, 0.0f, 1.0f, 0.0f),
        new(0.5f, 0.5f, 0.5f, 0.0f, 1.0f, 0.0f),
        new(-0.5f, 0.5f, 0.5f, 0.0f, 1.0f, 0.0f),
        new(-0.5f, 0.5f, -0.5f, 0.0f, 1.0f, 0.0f),
    };
}
