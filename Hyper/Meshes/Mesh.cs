﻿using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Hyper.Meshes
{
    public class Mesh : IDisposable
    {
        public int VaoId { get; set; }

        public int VboId { get; set; }

        public Texture? Texture { get; set; }

        public Vector3 Position { get; set; } = Vector3.Zero;
        //Will also have to add rotation and scale

        public int NumberOfVertices { get; set; } = 0;

        public Mesh(float[] vertices, Vector3 position)
        {
            CreateVertexArrayObject(vertices);
            Position = position;
            NumberOfVertices = vertices.Length / 6;
        }

        public virtual void Render(Shader shader, float scale, Vector3 cameraPosition)
        {
            var model = Matrix4.CreateTranslation((Position - cameraPosition) * scale);
            var scaleMatrix = Matrix4.CreateScale(scale);
            shader.SetMatrix4("model", scaleMatrix * model);

            GL.BindVertexArray(VaoId);
            GL.DrawArrays(PrimitiveType.Triangles, 0, NumberOfVertices);
        }

        private void CreateVertexArrayObject(float[] vertices)
        {
            int vaoId = GL.GenVertexArray();
            GL.BindVertexArray(vaoId);

            int vboId = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vboId);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
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
