using OpenTK.Mathematics;

namespace Hyper.Meshes;
internal class BoxMesh
{
    public static Mesh Create(Vector3 scaling, Vector3 position)
    {
        Mesh cube = CubeMesh.Create(position);
        Vertex[] vertices = cube.Vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i].X *= scaling.X;
            vertices[i].Y *= scaling.Y;
            vertices[i].Z *= scaling.Z;
        }

        Mesh mesh = new Mesh(vertices, position);
        mesh.Scaling = scaling;

        return mesh;
    }

    public static Mesh Create(Vector3 scaling)
        => Create(scaling, Vector3.Zero);
}
