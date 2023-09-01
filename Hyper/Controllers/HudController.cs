using Common;
using Hud;
using Hud.HUDElements;
using Hyper.PlayerData.InventorySystem.InventoryRendering;
using Hyper.Shaders;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;

namespace Hyper.Controllers;

internal class HudController : IController
{
    private readonly Shader _shader;

    private readonly IHudElement[] _elements;
    
    private readonly GameWindow _window;

    // TODO fix fps counter and fix the offset on item moving.
    public HudController(GameWindow window, Shader shader)
    {
        _window = window;
        _shader = shader;
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
        ShaderFactory.SetUpHudShaderParams(_shader, _window.Size.X / (float)_window.Size.Y);

        foreach (var element in _elements)
        {
            if (element.Visible) element.Render(_shader);
        }
        GL.Enable(EnableCap.DepthTest);
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
