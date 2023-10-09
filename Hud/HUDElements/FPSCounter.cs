using System.Diagnostics;
using Common;
using OpenTK.Mathematics;

namespace Hud.HUDElements;

public class FpsCounter : IHudElement
{
    public bool Visible { get; set; } = true;

    private readonly float _size;

    private const double FpsTimeFrame = 0.07f;

    private readonly Stopwatch _stopwatch = new();

    private int _frameCount;

    private double _elapsedTime;

    private int _fps;

    private readonly IWindowHelper _windowHelper;

    public FpsCounter(IWindowHelper windowHelper)
    {
        _windowHelper = windowHelper;
        _size = 0.06f;
        _stopwatch.Start();
    }

    public void Render(Shader shader)
    {
        shader.SetVector4("color", Vector4.One);
        UpdateFps();

        Printer.RenderStringTopRight(shader, _fps.ToString(), _size, _windowHelper.GetAspectRatio() / 2, 0.5f);
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

    public void Dispose()
    {
        // Nothing to dispose
    }
}
