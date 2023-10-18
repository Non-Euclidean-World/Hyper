using Assimp;
using OpenTK.Graphics.OpenGL4;

namespace Common;

// basically a subset of ModelLoader, some stuff could be reused
internal class ObjectLoader
{
    public static Scene GetModel(string path)
    {
        AssimpContext importer = new AssimpContext();
        return importer.ImportFile(path);
    }

    public static int[] GetVaos(Scene model)
    {
        var vaos = new List<int>();

        foreach (var mesh in model.Meshes)
        {
            int vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);

            SetUpVertices(mesh);
            SetUpNormals(mesh);
            SetUpFaces(mesh);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);

            vaos.Add(vao);
        }

        return vaos.ToArray();
    }

    private static void SetUpVertices(Mesh mesh)
    {
        int vboPositions = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, vboPositions);

        var vertices = mesh.Vertices.SelectMany(v => new[] { v.X, v.Y, v.Z }).ToArray();
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
        GL.EnableVertexAttribArray(0);
    }

    private static void SetUpNormals(Mesh mesh)
    {
        int vboNormals = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, vboNormals);

        var normals = mesh.Normals.SelectMany(n => new[] { n.X, n.Y, n.Z }).ToArray();
        GL.BufferData(BufferTarget.ArrayBuffer, normals.Length * sizeof(float), normals, BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 0, 0);
        GL.EnableVertexAttribArray(1);
    }

    private static void SetUpFaces(Mesh mesh)
    {
        if (!mesh.HasFaces) return;

        int ebo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);

        var indices = mesh.Faces.SelectMany(f => f.Indices).ToArray();
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(int), indices, BufferUsageHint.StaticDraw);
    }
}
