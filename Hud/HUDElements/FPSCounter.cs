using System.Diagnostics;
using Common;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;

namespace Hud.HUDElements;

public class FpsCounter : IHudElement
{
    public bool Visible { get; set; } = true;
    
    private readonly Vector2 _size;

    private const double FpsTimeFrame = 0.1f;

    private readonly Stopwatch _stopwatch = new();

    private int _frameCount = 0;

    private double _elapsedTime = 0;

    private int _fps = 0;
    
    private readonly GameWindow _window;
    
    public FpsCounter(GameWindow window)
    {
        _window = window;
        _size = new Vector2(0.02f);
        _stopwatch.Start();
    }

    public void Render(Shader shader)
    {
        shader.SetVector4("color", Vector4.One);
        UpdateFps();

        Printer.RenderStringTopRight(shader, _fps.ToString(), _size.X, (float)_window.Size.X / _window.Size.Y / 2, 0.5f);
    }

    private void UpdateFps()
    {
        _frameCount++;
        _elapsedTime = _stopwatch.Elapsed.TotalSeconds;
        if (_elapsedTime >= FpsTimeFrame)
        {
            _fps = (int)(_frameCount / _elapsedTime);
            _frameCount = 0;
            _stopwatch.Restart();
        }
    }
}
