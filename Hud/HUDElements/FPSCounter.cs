﻿using System.Diagnostics;
using Common;
using OpenTK.Mathematics;

namespace Hud.HUDElements;
/// <summary>
/// Displays the current FPS in the top right corner of the screen.
/// </summary>
public class FpsCounter : IHudElement
{
    public bool Visible { get; set; } = true;

    private const float Size = 0.06f;

    private const double FpsTimeFrame = 0.1f;

    private readonly Stopwatch _stopwatch = new();

    private int _frameCount;

    private double _elapsedTime;

    private int _fps;

    private readonly IWindowHelper _windowHelper;

    /// <summary>
    /// Creates a new instance of the <see cref="FpsCounter"/> class.
    /// </summary>
    /// <param name="windowHelper"></param>
    public FpsCounter(IWindowHelper windowHelper)
    {
        _windowHelper = windowHelper;
        _stopwatch.Start();
    }

    public void Render(Shader shader)
    {
        shader.SetVector4("color", Vector4.One);
        UpdateFps();

        Printer.RenderStringTopRight(shader, _fps.ToString(), Size, _windowHelper.GetAspectRatio() / 2, 0.5f);
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
