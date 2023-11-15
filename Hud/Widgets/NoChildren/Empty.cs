using OpenTK.Mathematics;

namespace Hud.Widgets.NoChildren;

/// <summary>
/// A widget that does nothing.
/// </summary>
public class Empty : Widget
{
    public override Vector2 GetSize()
    {
        return Vector2.Zero;
    }

    public override void Render(Context context)
    {

    }
}