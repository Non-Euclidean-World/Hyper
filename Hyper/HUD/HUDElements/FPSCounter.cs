using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Diagnostics;

namespace Hyper.HUD.HUDElements
{
    internal class FPSCounter : HUDElement
    {
        internal const float DefaultSize = 0.02f;

        internal static Vector2 DefaultPosition = new Vector2(0.64f, 0.48f);

        private const double _fpsTimeFrame = 0.1f;

        private static float[] _vertices = {
            // Position    Color             Texcoords
            -1f,  1f,  0f, 0f, 1f, 0f, 0f,  // Top-left
             1f,  1f,  0f, 0f, 1f, 1f, 0f,  // Top-right
             1f, -1f,  0f, 0f, 1f, 1f, 1f,  // Bottom-right
             1f, -1f,  0f, 0f, 1f, 1f, 1f,  // Bottom-right
            -1f, -1f,  0f, 0f, 1f, 0f, 1f,  // Bottom-left
            -1f,  1f,  0f, 0f, 1f, 0f, 0f,  // Top-left
        };

        private Texture[] _numberTextures;

        private Stopwatch _stopwatch = new Stopwatch();

        private int _frameCount = 0;

        private double _elapsedTime = 0;

        private int _fps = 0;

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

            _stopwatch.Start();
        }

        public override void Render(Shader shader)
        {
            shader.SetBool("useTexture", true);
            GL.BindVertexArray(_vaoId);

            UpdateFPS();
            RenderNumber(shader, _fps);
        }

        private void RenderNumber(Shader shader, int number)
        {
            float offset = 0;
            while (number > 0)
            {
                int digit = number % 10;
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
            _frameCount++;
            _elapsedTime = _stopwatch.Elapsed.TotalSeconds;
            if (_elapsedTime >= _fpsTimeFrame)
            {
                _fps = (int)(_frameCount / _elapsedTime);
                _frameCount = 0;
                _stopwatch.Restart();
            }
        }
    }
}
