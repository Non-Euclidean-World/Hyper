using Hyper.Command;
using Hyper.HUD.HUDElements;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Hyper.HUD
{
    internal class HUDManager : Commandable
    {
        private Shader _shader;

        private Dictionary<HUDElementTypes, HUDElement> _elements;

        public HUDManager()
        {
            _shader = CreateShader();
            _elements = new Dictionary<HUDElementTypes, HUDElement>()
            {
                { HUDElementTypes.Crosshair, new Crosshair(new Vector2(0, 0), Crosshair.DefaultSize) },
                { HUDElementTypes.FPSCounter, new FPSCounter(FPSCounter.DefaultPosition, FPSCounter.DefaultSize) },
            };
        }

        public void Render(float aspectRatio)
        {
            var projection = Matrix4.CreateOrthographic(aspectRatio, 1, -1.0f, 1.0f);

            _shader.Use();

            _shader.SetMatrix4("projection", projection);

            foreach (var element in _elements)
            {
                if (element.Value.Visible) element.Value.Render(_shader);
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

        protected override void SetCommand(string[] args)
        {
            switch (args[0])
            {
                case "crosshair":
                    SetVisibility(args[1], HUDElementTypes.Crosshair);
                    break;
                case "fps":
                    SetVisibility(args[1], HUDElementTypes.FPSCounter);
                    break;
                default:
                    throw new CommandException($"Property '{args[0]}' not found");
            }
        }

        protected override void GetCommand(string[] args)
        {
            switch (args[0])
            {
                case "crosshair":
                    if (args[1] == "visibility") Console.WriteLine(_elements[HUDElementTypes.Crosshair].Visible);
                    break;
                case "fps":
                    if (args[1] == "visibility") Console.WriteLine(_elements[HUDElementTypes.FPSCounter].Visible);
                    break;
                default:
                    throw new CommandException($"Property '{args[0]}' not found");
            }
        }

        private void SetVisibility(string argument, HUDElementTypes elementType)
        {
            if (argument == "visible") _elements[elementType].Visible = true;
            else if (argument == "invisible") _elements[elementType].Visible = false;
            else CommandNotFound();
        }
    }
}
