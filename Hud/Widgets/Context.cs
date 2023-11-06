using Hud.Shaders;
using OpenTK.Mathematics;

namespace Hud.Widgets;

/// <summary>
/// This describes how much space there is on the screen.
/// </summary>
/// <param name="Shader"></param>
/// <param name="Position">The top left corner of the Context</param>
/// <param name="Size">Width and height of the Context</param>
public record Context(HudShader Shader, Vector2 Position, Vector2 Size)
{
    public Context(Context oldContext, Vector2 newPosition) : 
        this(oldContext.Shader, newPosition, oldContext.Size) { }
    
    public Context(Context oldContext, Vector2 newPosition, Vector2 newSize) :
        this(oldContext.Shader,  newPosition, newSize) { }
}
