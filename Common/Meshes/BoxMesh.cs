using OpenTK.Mathematics;

namespace Common.Meshes;

/// <summary>
/// A utility class for creating box meshes.
/// </summary>
public static class BoxMesh
{
    /// <summary>
    /// Creates a box mesh with the specified size, position, and color.
    /// </summary>
    /// <param name="size">The size of the box (width, height, depth).</param>
    /// <param name="position">The position of the box in 3D space.</param>
    /// <param name="color">The color of the box in RGB format (0-1 range).</param>
    /// <returns>A mesh representing a box with the specified parameters.</returns>
    public static Mesh Create(Vector3 size, Vector3 position, Vector3 color)
    {
        Vertex[] vertices = new Vertex[CubeMesh.Vertices.Length];
        Array.Copy(CubeMesh.Vertices, vertices, CubeMesh.Vertices.Length);

        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i].X *= size.X;
            vertices[i].Y *= size.Y;
            vertices[i].Z *= size.Z;

            vertices[i].R = color.X;
            vertices[i].G = color.Y;
            vertices[i].B = color.Z;
        }

        Mesh mesh = new Mesh(vertices, position);

        return mesh;
    }

    /// <summary>
    /// Creates a box mesh with the specified size at the origin (0, 0, 0) with default color (white).
    /// </summary>
    /// <param name="size">The size of the box (width, height, depth).</param>
    /// <returns>A mesh representing a box with the specified size and default position and color.</returns>
    public static Mesh Create(Vector3 size)
        => Create(size, Vector3.Zero, Vector3.One);

    /// <summary>
    /// Creates a box mesh with the specified size, at the origin (0, 0, 0), and the specified color.
    /// </summary>
    /// <param name="size">The size of the box (width, height, depth).</param>
    /// <param name="color">The color of the box in RGB format.</param>
    /// <returns>A mesh representing a box with the specified size, default position, and specified color.</returns>
    public static Mesh Create(Vector3 size, Vector3 color)
        => Create(size, Vector3.Zero, color);
}
