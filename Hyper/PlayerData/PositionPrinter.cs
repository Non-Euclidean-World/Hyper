using Common;
using Hud;
using OpenTK.Mathematics;

namespace Hyper.PlayerData;

internal class PositionPrinter : IHudElement
{
    private readonly Camera _camera;
    
    private readonly IWindowHelper _windowHelper;
    
    private const float Size = 0.03f;
    
    public bool Visible { get; set; }
    
    public PositionPrinter(Camera camera, IWindowHelper windowHelper)
    {
        _camera = camera;
        _windowHelper = windowHelper;
        Visible = true;
    }
    
    public void Render(Shader shader)
    {
        shader.SetVector4("color", Vector4.One);
        Printer.RenderStringTopLeft(shader, GetPositionString(), Size, -_windowHelper.GetAspectRatio() / 2, 0.5f);
    }
    
    private string GetPositionString()
    {
        var position = _camera.ReferencePointPosition;
        return $"X:{Math.Round(position.X, 1)}" +
               $"\nY:{Math.Round(position.Y, 1)}" +
               $"\nZ:{Math.Round(position.Z, 1)}";
    }
    
    public void Dispose()
    {
        // Nothing to dispose
    }
}