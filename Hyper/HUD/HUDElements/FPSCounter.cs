using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hyper.HUD.HUDElements
{
    internal class FPSCounter : HUDElement
    {
        private static float[] _vertices = {
            // Position    Color             Texcoords
            -1f,  1f,  0.0f, 0.0f, 1.0f, 0.0f, 0.0f,  // Top-left
             1f,  1f,  0.0f, 0.0f, 1.0f, 1.0f, 0.0f,  // Top-right
             1f, -1f,  0.0f, 0.0f, 1.0f, 1.0f, 1.0f,  // Bottom-right
             1f, -1f,  0.0f, 0.0f, 1.0f, 1.0f, 1.0f,  // Bottom-right
            -1f, -1f,  0.0f, 0.0f, 1.0f, 0.0f, 1.0f,  // Bottom-left
            -1f,  1f,  0.0f, 0.0f, 1.0f, 0.0f, 0.0f,  // Top-left
        };

        private Texture[] _numberTextures;

        private Stopwatch stopwatch = new Stopwatch();

        private int frameCount = 0;

        private double elapsedTime = 0;

        private int fps = 0;

        public FPSCounter(Vector2 position, float size) : base(position, size, _vertices)
        {
            _numberTextures = new[] {
                Texture.LoadFromText("0"),
                Texture.LoadFromText("1"),
                Texture.LoadFromText("2"),
                Texture.LoadFromText("3"),
                Texture.LoadFromText("4"),
                Texture.LoadFromText("5"),
                Texture.LoadFromText("6"),
                Texture.LoadFromText("7"),
                Texture.LoadFromText("8"),
                Texture.LoadFromText("9"),
            };

            stopwatch.Start();
        }

        public override void Render(Shader shader)
        {
            shader.SetBool("useTexture", true);
            GL.BindVertexArray(_vaoId);
            
            UpdateFPS();
            RenderNumber(shader, fps);
        }

        private void RenderNumber(Shader shader, int number)
        {
            float offset = 0;
            while (number > 0)
            {
                var digit = number % 10;
                RenderSingleDigit(shader, digit, offset);
                offset -= _size * 2;
                number /= 10;
            }
        }

        private void RenderSingleDigit(Shader shader, int digit, float offset)
        {
            var model = Matrix4.CreateTranslation(_position.X + offset, _position.Y, 0.0f);
            model = Matrix4.CreateScale(_size, _size, 1.0f) * model;
            shader.SetMatrix4("model", model);
            _numberTextures[digit].Use(TextureUnit.Texture0);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
        }

        private void UpdateFPS()
        {
            frameCount++;
            elapsedTime = stopwatch.Elapsed.TotalSeconds;
            if (elapsedTime >= 0.1)
            {
                fps = (int) (frameCount / elapsedTime);
                frameCount = 0;
                stopwatch.Restart();
            }
        }
    }
}
