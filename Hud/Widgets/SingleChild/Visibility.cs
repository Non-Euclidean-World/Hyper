
using OpenTK.Mathematics;

namespace Hud.Widgets.SingleChild;
/// <summary>
/// A widget that can hide its child.
/// </summary>
public class Visibility : SingleChildWidget
{
    public bool Visible;
    
    /// <summary>
    /// Creates an instance of Visibility class.
    /// </summary>
    /// <param name="child"></param>
    /// <param name="visible"></param>
    public Visibility(Widget child, bool visible = true)
    {
        Child = child;
        Visible = visible;
    }
    
    public override void Render(Context context)
    {
        if (Visible)
            Child.Render(context);
    }
    
    public override Vector2 GetSize()
    {
        return Visible ? Child.GetSize() : Vector2.Zero;
    }
}