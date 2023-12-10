using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

namespace Common;

/// <summary>
/// Represents a helper interface for window-related operations and properties.
/// </summary>
public interface IWindowHelper
{
    /// <summary>
    /// Gets or sets the state of the cursor.
    /// </summary>
    CursorState CursorState { get; set; }

    /// <summary>
    /// Retrieves the current mouse position relative to the window.
    /// </summary>
    /// <returns>The current mouse position as a 2D vector.</returns>
    Vector2 GetMousePosition();

    /// <summary>
    /// Retrieves the aspect ratio of the window.
    /// </summary>
    /// <returns>The aspect ratio of the window as a floating-point value.</returns>
    float GetAspectRatio();
}