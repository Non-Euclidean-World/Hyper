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
            // Position // Color // Texture Coords
             -1f, 0f, 1.0f, 0.0f, 0.0f, 0f, 0f,
             1f, 0f, 1.0f, 0.0f, 0.0f, 0f, 0f,
             0.0f, 1f, 1.0f, 0.0f, 0.0f, 0f, 0f,
             0.0f, -1f, 1.0f, 0.0f, 0.0f, 0f, 0f
        };

        public Crosshair(Vector2 position, float size) : base(position, size, vertices) { }

        public override void Render(Shader shader)
        {
            var model = Matrix4.CreateTranslation(_position.X, _position.Y, 0.0f);
            model *= Matrix4.CreateScale(_size, _size, 1.0f);

            shader.SetMatrix4("model", model);
            shader.SetBool("useTexture", false);
            GL.BindVertexArray(_vaoId);
            GL.DrawArrays(PrimitiveType.Lines, 0, 4);
        }
    }
}
