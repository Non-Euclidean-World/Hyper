using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Common.Meshes;

// <summary>
/// Represents a mesh consisting of vertices for rendering in a 3D environment.
/// </summary>
public class Mesh
{
    /// <summary>
    /// The position of the mesh in 3D space.
    /// </summary>
    public Vector3 Position { get; set; }

    /// <summary>
    /// The Vertex Array Object (VAO) ID used for OpenGL rendering.
    /// </summary>
    public int VaoId;

    /// <summary>
    /// The Vertex Buffer Object (VBO) ID used for OpenGL rendering.
    /// </summary>
    protected int VboId;

    /// <summary>
    /// The array of vertices comprising the mesh.
    /// </summary>
    public Vertex[] Vertices { get; set; }

    public Mesh(Vertex[] vertices, Vector3 position, bool createVertexArrayObject = true)
    {
        Vertices = vertices;
        if (createVertexArrayObject) CreateVertexArrayObject();
        Position = position;
    }

    /// <summary>
    /// Constructs a <see cref="Mesh"/> object with the given vertices and position.
    /// </summary>
    /// <param name="vertices">The array of vertices defining the mesh.</param>
    /// <param name="position">The position of the mesh in 3D space.</param>
    /// <param name="createVertexArrayObject">Determines if a Vertex Array Object (VAO) should be created.</param>
    public virtual void Render(Shader shader, float scale, float curve, Vector3 cameraPosition)
    {
        var model = Matrix4.CreateTranslation(GeomPorting.CreateTranslationTarget(Position, cameraPosition, curve, scale));
        var scaleMatrix = Matrix4.CreateScale(scale);
        shader.SetMatrix4("model", scaleMatrix * model);
        shader.SetMatrix4("normalRotation", Matrix4.Identity);

        GL.BindVertexArray(VaoId);
        GL.DrawArrays(PrimitiveType.Triangles, 0, Vertices.Length);
    }

    /// <summary>
    /// Creates a Vertex Array Object (VAO) for the mesh and sets up vertex attribute pointers.
    /// </summary>
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

    /// <summary>
    /// Updates the mesh's vertex buffer data in the GPU.
    /// </summary>
    public void Update()
    {
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

    /// <summary>
    /// Disposes of allocated OpenGL resources associated with the mesh.
    /// </summary>
    public void Dispose()
    {
        GL.DeleteBuffer(VboId);
        GL.DeleteVertexArray(VaoId);
    }
}
