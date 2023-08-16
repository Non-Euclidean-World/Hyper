using Hyper.Command;
using Hyper.HUD.HUDElements;
using Hyper.HUD.HUDElements.InventoryRendering;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Hyper.HUD;

internal class HudManager : Commandable
{
    public float AspectRatio { get; set; }

    private readonly Shader _shader;

    private readonly IHudElement[] _elements;

    public HudManager(float aspectRatio)
    {
        AspectRatio = aspectRatio;
        _shader = CreateShader();
        _elements = new IHudElement[]
        {
            new Crosshair(),
            new FpsCounter(),
            new InventoryRenderer(),
        };
    }

    public void Render()
    {
        GL.Disable(EnableCap.DepthTest);
        var projection = Matrix4.CreateOrthographic(AspectRatio, 1, -1.0f, 1.0f);

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
            ("HUD/Shaders/shader2d.vert", ShaderType.VertexShader),
            ("HUD/Shaders/shader2d.frag", ShaderType.FragmentShader)
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
