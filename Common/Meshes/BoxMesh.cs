using OpenTK.Mathematics;

namespace Common.Meshes;
public static class BoxMesh
{
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

    public static Mesh Create(Vector3 size)
        => Create(size, Vector3.Zero, Vector3.One);

    public static Mesh Create(Vector3 size, Vector3 color)
        => Create(size, Vector3.Zero, color);
}
