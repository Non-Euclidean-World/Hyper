using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Common.Meshes;

public class Mesh
{
    public Vector3 Position { get; set; }

    public int VaoId;

    protected int VboId;

    public int NumberOfVertices;

    public Vertex[] Vertices { get; set; }

    public Mesh(Vertex[] vertices, Vector3 position, bool createVertexArrayObject = true)
    {
        Vertices = vertices;
        if (createVertexArrayObject) CreateVertexArrayObject();
        Position = position;
        NumberOfVertices = Vertices.Length;
    }

    public virtual void Render(Shader shader, float scale, Vector3 cameraPosition)
    {
        var model = Matrix4.CreateTranslation((Position - cameraPosition) * scale);
        var scaleMatrix = Matrix4.CreateScale(scale);
        shader.SetMatrix4("model", scaleMatrix * model);

        GL.BindVertexArray(VaoId);
        GL.DrawArrays(PrimitiveType.Triangles, 0, NumberOfVertices);
    }

    public void CreateVertexArrayObject()
    {
        int vaoId = GL.GenVertexArray();
        GL.BindVertexArray(vaoId);

        int vboId = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, vboId);
        GL.BufferData(BufferTarget.ArrayBuffer, Vertices.Length * Marshal.SizeOf<Vertex>(), Vertices, BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Marshal.SizeOf<Vertex>(), 0);
        GL.EnableVertexAttribArray(0);

        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, Marshal.SizeOf<Vertex>(), 3 * sizeof(float));
        GL.EnableVertexAttribArray(1);

        GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, Marshal.SizeOf<Vertex>(), 6 * sizeof(float));
        GL.EnableVertexAttribArray(2);

        GL.BindVertexArray(0);

        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

        VaoId = vaoId;
        VboId = vboId;
    }
    
    public void Update(Vertex[] vertices)
    {
        Vertices = vertices;
        NumberOfVertices = Vertices.Length;

        GL.BindVertexArray(VaoId);
        GL.BindBuffer(BufferTarget.ArrayBuffer, VboId);
        GL.BufferData(BufferTarget.ArrayBuffer, Vertices.Length * Marshal.SizeOf<Vertex>(), IntPtr.Zero, BufferUsageHint.StaticDraw);
        IntPtr ptr = GL.MapBuffer(BufferTarget.ArrayBuffer, BufferAccess.WriteOnly);
        unsafe
        {
            fixed (Vertex* source = Vertices)
            {
                System.Buffer.MemoryCopy(source, ptr.ToPointer(), Vertices.Length * Marshal.SizeOf<Vertex>(), Vertices.Length * Marshal.SizeOf<Vertex>());
            }
        }
        GL.UnmapBuffer(BufferTarget.ArrayBuffer);
    }

    public void Dispose()
    {
        GL.DeleteBuffer(VboId);
        GL.DeleteVertexArray(VaoId);
    }
}
