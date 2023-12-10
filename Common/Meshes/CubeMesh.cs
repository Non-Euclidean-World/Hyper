namespace Common.Meshes;

/// <summary>
/// A utility class for creating cube meshes.
/// </summary>
public static class CubeMesh
{
    /// <summary>
    /// Defines the vertices for a cube mesh.
    /// </summary>
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
