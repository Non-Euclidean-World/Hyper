using System.Diagnostics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Hyper.HUD.HUDElements;

internal class FpsCounter : HudElement
{
    public const float DefaultSize = 0.02f;

    public static readonly Vector2 DefaultPosition = new(0.64f, 0.48f);

    private const double FpsTimeFrame = 0.1f;

    private readonly Stopwatch _stopwatch = new();

    private int _frameCount = 0;

    private double _elapsedTime = 0;

    private int _fps = 0;

    public FpsCounter(Vector2 position, float size) : base(position, size)
    {
        _stopwatch.Start();
    }

    public override void Render(Shader shader)
    {
        UpdateFps();
        Printer.RenderString(shader, _fps.ToString(), Size, Position.X - 0.1f, Position.Y);
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
