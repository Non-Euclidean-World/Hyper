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
                new Crosshair(new Vector2(0, 0), 0.03f)
            };
        }

        public void Render(float aspectRatio)
        {
            Matrix4 projection;
            if (aspectRatio > 1.0f) projection = Matrix4.CreateOrthographicOffCenter(-aspectRatio, aspectRatio, -1.0f, 1.0f, -1.0f, 1.0f);
            else projection = Matrix4.CreateOrthographicOffCenter(-1.0f, 1.0f, -1 / aspectRatio, 1 / aspectRatio, -1.0f, 1.0f);

            Matrix4 view = Matrix4.Identity;
            Matrix4 model = Matrix4.Identity;

            _shader.Use();

            _shader.SetMatrix4("projection", projection);
            _shader.SetMatrix4("view", view);

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
