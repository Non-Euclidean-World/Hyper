using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

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
            // Create VAO
            int vaoId = GL.GenVertexArray();
            GL.BindVertexArray(vaoId);

            // Create VBO for vertices
            int vboId = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vboId);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * Marshal.SizeOf<Vertex>(), vertices, BufferUsageHint.StaticDraw);

            // Define vertex attributes
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Marshal.SizeOf<Vertex>(), 0); // Position attribute
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, Marshal.SizeOf<Vertex>(), Vector3.SizeInBytes); // Texture coordinate attribute
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);

            // Create EBO for indices
            int eboId = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, eboId);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(int), indices, BufferUsageHint.StaticDraw);

            // Unbind the VAO first (Important Note: EBO should be unbound after VAO, so VAO remembers EBO.)
            GL.BindVertexArray(0);

            // Unbind other resources (Optional, but a good practice)
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            return vaoId;
        }
    }
}
