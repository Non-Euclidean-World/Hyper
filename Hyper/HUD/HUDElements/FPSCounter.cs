using System.Diagnostics;
using OpenTK.Mathematics;

namespace Hyper.HUD.HUDElements;

internal class FpsCounter : IHudElement
{
    public bool Visible { get; set; } = true;
    
    private readonly Vector2 _size;

    private readonly Vector2 _position;

    private const double FpsTimeFrame = 0.1f;

    private readonly Stopwatch _stopwatch = new();

    private int _frameCount = 0;

    private double _elapsedTime = 0;

    private int _fps = 0;

    public FpsCounter()
    {
        _position = new Vector2(0.64f, 0.48f);
        _size = new Vector2(0.02f);
        _stopwatch.Start();
    }

    public void Render(Shader shader)
    {
        UpdateFps();
        Printer.RenderString(shader, _fps.ToString(), _size.X, _position.X - 0.1f, _position.Y);
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
