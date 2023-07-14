using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hyper.HUD.HUDElements
{
    internal class Crosshair : HUDElement
    {
        private static float[] vertices = {
             -1f, 0f, 1.0f, 0.0f, 0.0f,
             1f, 0f, 1.0f, 0.0f, 0.0f,
             0.0f, 1f, 1.0f, 0.0f, 0.0f,
             0.0f, -1f, 1.0f, 0.0f, 0.0f
        };

        public Crosshair(Vector2 position, float size) : base(position, size)
        {
            int vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);

            int vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);

            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 2 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);

            _vaoId = vao;
        }

        public override void Render(Shader shader)
        {
            var model = Matrix4.CreateTranslation(_position.X, _position.Y, 0.0f);
            model *= Matrix4.CreateScale(_size, _size, 1.0f);

            shader.SetMatrix4("model", model);
            GL.BindVertexArray(_vaoId);
            GL.DrawArrays(PrimitiveType.Lines, 0, 6);
        }
    }
}
