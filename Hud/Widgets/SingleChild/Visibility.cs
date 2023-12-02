
using OpenTK.Mathematics;

namespace Hud.Widgets.SingleChild;

public class Visibility : SingleChildWidget
{
    public bool Visible;
    
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