using Hyper.HUD.HUDElements;
using Hyper.Meshes;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Hyper.HUD
{
    internal class HUDManager
    {
        private Shader _shader;

        private List<HUDElement> _elements;

        public HUDManager()
        {
            _shader = CreateShader();
            _elements = new List<HUDElement>()
            {
                new Crosshair(new Vector2(0, 0), 0.02f),
                new FPSCounter(new Vector2(0.64f, 0.48f), 0.02f)
            };
        }

        public void Render(float aspectRatio)
        {
            var projection = Matrix4.CreateOrthographic(aspectRatio, 1, -1.0f, 1.0f);

            _shader.Use();

            _shader.SetMatrix4("projection", projection);

            foreach (var element in _elements)
            {
                element.Render(_shader);
            }
        }

        private Shader CreateShader()
        {
            var shader = new (string, ShaderType)[]
            {
                ("HUD/Shaders/shader2d.vert", ShaderType.VertexShader),
                ("HUD/Shaders/shader2d.frag", ShaderType.FragmentShader)
            };

            return new Shader(shader);
        }
    }
}
