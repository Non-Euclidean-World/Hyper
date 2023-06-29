using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Runtime.InteropServices;

namespace Hyper.Meshes
{
    public class Mesh
    {
        public int VaoId { get; set; }

        public Texture? Texture { get; set; }

        public Vector3 Position { get; set; } = Vector3.Zero;
        //Will also have to add rotation and scale

        public int numberOfIndices { get; set; }

        public Mesh(Vertex[] vertices, int[] indices)
        {
            VaoId = CreateVertexArrayObject(vertices, indices);
            numberOfIndices = indices.Length;
        }

        public Mesh(Vertex[] vertices, int[] indices, Vector3 position)
        {
            VaoId = CreateVertexArrayObject(vertices, indices);
            numberOfIndices = indices.Length;
            Position = position;
        }

        private int CreateVertexArrayObject(Vertex[] vertices, int[] indices)
        {
            int vaoId = GL.GenVertexArray();
            GL.BindVertexArray(vaoId);

            int vboId = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vboId);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * Marshal.SizeOf<Vertex>(), vertices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Marshal.SizeOf<Vertex>(), 0);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, Marshal.SizeOf<Vertex>(), Vector3.SizeInBytes);
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);

            int eboId = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, eboId);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(int), indices, BufferUsageHint.StaticDraw);

            GL.BindVertexArray(0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            return vaoId;
        }
    }
}
