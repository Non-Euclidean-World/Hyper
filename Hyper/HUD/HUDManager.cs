using Common;
using Common.Command;
using Hud;
using Hud.HUDElements;
using Hyper.HUD.InventoryRendering;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;

namespace Hyper.HUD;

internal class HudManager : Commandable
{
    private readonly Shader _shader;

    private readonly IHudElement[] _elements;
    
    private readonly GameWindow _window;

    // TODO fix fps counter and fix the offset on item moving.
    public HudManager(GameWindow window)
    {
        _window = window;
        _shader = CreateShader();
        _elements = new IHudElement[]
        {
            new Crosshair(),
            new FpsCounter(window),
            new InventoryRenderer(),
        };
    }

    public void Render()
    {
        GL.Disable(EnableCap.DepthTest);
        var projection = Matrix4.CreateOrthographic(_window.Size.X / (float)_window.Size.Y, 1, -1.0f, 1.0f);

        _shader.Use();

        _shader.SetMatrix4("projection", projection);

        foreach (var element in _elements)
        {
            if (element.Visible) element.Render(_shader);
        }
        GL.Enable(EnableCap.DepthTest);
    }

    private Shader CreateShader()
    {
        var shader = new []
        {
            (@"..\\..\\..\\..\\Hud\\bin\\Debug\\net7.0\\Shaders\\shader2d.vert", ShaderType.VertexShader),
            (@"..\\..\\..\\..\\Hud\\bin\\Debug\\net7.0\\Shaders\\shader2d.frag", ShaderType.FragmentShader)
        };

        return new Shader(shader);
    }
    
    public static Vector2 GetMousePosition()
    {
        var window = Window.Instance;
        var mouse = window.MouseState;
        var windowSize = window.Size;
        var aspectRatio = windowSize.X / (float)windowSize.Y;
        
        return new Vector2(
            (mouse.X / windowSize.X) * aspectRatio - aspectRatio * 0.5f,
            0.5f - (mouse.Y / windowSize.Y));
    }
}
