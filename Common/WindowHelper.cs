using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace Common;

public class WindowHelper : IWindowHelper
{
    private readonly GameWindow _window;

    public CursorState CursorState
    {
        get => _window.CursorState;
        set => _window.CursorState = value;
    }

    public WindowHelper(GameWindow window)
    {
        _window = window;
    }

    public Vector2 GetMousePosition()
    {
        var mouse = _window.MouseState;
        var windowSize = _window.Size;
        var aspectRatio = windowSize.X / (float)windowSize.Y;

        return new Vector2(
            (mouse.X / windowSize.X) * aspectRatio - aspectRatio * 0.5f,
            0.5f - (mouse.Y / windowSize.Y));
    }

    public float GetAspectRatio() => (float)_window.Size.X / _window.Size.Y;
}