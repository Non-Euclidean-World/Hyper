using Hyper.Command;
using Hyper.HUD.HUDElements;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Hyper.HUD
{
    internal class HudManager : Commandable
    {
        internal float AspectRatio;

        private readonly Shader _shader;

        private readonly Dictionary<HudElementTypes, HudElement> _elements;

        public HudManager(float aspectRatio)
        {
            AspectRatio = aspectRatio;
            _shader = CreateShader();
            _elements = new Dictionary<HudElementTypes, HudElement>()
            {
                { HudElementTypes.Crosshair, new Crosshair(new Vector2(0, 0), Crosshair.DefaultSize) },
                { HudElementTypes.FpsCounter, new FpsCounter(FpsCounter.DefaultPosition, FpsCounter.DefaultSize) },
            };
        }

        public void Render()
        {
            var projection = Matrix4.CreateOrthographic(AspectRatio, 1, -1.0f, 1.0f);

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
                    SetVisibility(args[1], HudElementTypes.Crosshair);
                    break;
                case "fps":
                    SetVisibility(args[1], HudElementTypes.FpsCounter);
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
                    if (args[1] == "visibility") Console.WriteLine(_elements[HudElementTypes.Crosshair].Visible);
                    break;
                case "fps":
                    if (args[1] == "visibility") Console.WriteLine(_elements[HudElementTypes.FpsCounter].Visible);
                    break;
                default:
                    throw new CommandException($"Property '{args[0]}' not found");
            }
        }

        private void SetVisibility(string argument, HudElementTypes elementType)
        {
            if (argument == "visible") _elements[elementType].Visible = true;
            else if (argument == "invisible") _elements[elementType].Visible = false;
            else CommandNotFound();
        }
    }
}
