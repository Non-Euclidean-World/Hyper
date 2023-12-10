using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace Common;

/// <summary>
/// Provides utility methods and properties for managing window-related operations.
/// Implements the <see cref="IWindowHelper"/> interface.
/// </summary>
public class WindowHelper : IWindowHelper
{
    private readonly GameWindow _window;

    /// <summary>
    /// Gets or sets the cursor state.
    /// </summary>
    public CursorState CursorState
    {
        get => _window.CursorState;
        set => _window.CursorState = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowHelper"/> class with the specified <see cref="GameWindow"/> object.
    /// </summary>
    /// <param name="window">The <see cref="GameWindow"/> object to manage window-related operations</param>
    public WindowHelper(GameWindow window)
    {
        _window = window;
    }

    /// <summary>
    /// Retrieves the current mouse position within the window's coordinate system.
    /// </summary>
    /// <returns>A Vector2 representing the mouse position</returns>
    public Vector2 GetMousePosition()
    {
        var mouse = _window.MouseState;
        var windowSize = _window.Size;
        var aspectRatio = windowSize.X / (float)windowSize.Y;

        return new Vector2(
            (mouse.X / windowSize.X) * aspectRatio - aspectRatio * 0.5f,
            0.5f - (mouse.Y / windowSize.Y));
    }

    /// <summary>
    /// Retrieves the aspect ratio of the window.
    /// </summary>
    /// <returns>The aspect ratio of the window</returns>
    public float GetAspectRatio() => (float)_window.Size.X / _window.Size.Y;
}