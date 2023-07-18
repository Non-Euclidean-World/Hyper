using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Hyper.Meshes
{
    internal class Mesh : IDisposable
    {
        internal Vector3 Position;
        //Will also have to add rotation and scale

        protected int VaoId;

        protected int VboId;

        protected int NumberOfVertices;

        internal Mesh(Vertex[] vertices, Vector3 position)
        {
            CreateVertexArrayObject(vertices);
            Position = position;
            NumberOfVertices = vertices.Length;
        }

        internal virtual void Render(Shader shader, float scale, Vector3 cameraPosition)
        {
            var model = Matrix4.CreateTranslation((Position - cameraPosition) * scale);
            var scaleMatrix = Matrix4.CreateScale(scale);
            shader.SetMatrix4("model", scaleMatrix * model);

            GL.BindVertexArray(VaoId);
            GL.DrawArrays(PrimitiveType.Triangles, 0, NumberOfVertices);
        }

        private void CreateVertexArrayObject(Vertex[] vertices)
        {
            int vaoId = GL.GenVertexArray();
            GL.BindVertexArray(vaoId);

            int vboId = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vboId);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * Marshal.SizeOf<Vertex>(), vertices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Marshal.SizeOf<Vertex>(), 0);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, Marshal.SizeOf<Vertex>(), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            GL.BindVertexArray(0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            VaoId = vaoId;
            VboId = vboId;
        }

        public void Dispose()
        {
            GL.DeleteVertexArray(VaoId);
        }
    }
}
