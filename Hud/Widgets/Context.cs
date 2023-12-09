using Hud.Shaders;
using OpenTK.Mathematics;

namespace Hud.Widgets;

/// <summary>
/// This describes how much space there is on the screen.
/// </summary>
/// <param name="Shader">The shader used to render widgets.</param>
/// <param name="Position">The top left corner of the Context</param>
/// <param name="Size">Width and height of the Context</param>
public record Context(HudShader Shader, Vector2 Position, Vector2 Size)
{
    /// <summary>
    /// Creates a new Context with the same shader and size but a different position.
    /// </summary>
    /// <param name="oldContext">The old context.</param>
    /// <param name="newPosition">The new position.</param>
    public Context(Context oldContext, Vector2 newPosition) :
        this(oldContext.Shader, newPosition, oldContext.Size)
    { }

    /// <summary>
    /// Creates a new Context with the same shader but a position and a different size.
    /// </summary>
    /// <param name="oldContext">The old context.</param>
    /// <param name="newPosition">The new position.</param>
    /// <param name="newSize">The new size.</param>
    public Context(Context oldContext, Vector2 newPosition, Vector2 newSize) :
        this(oldContext.Shader, newPosition, newSize)
    { }
}
