using OpenTK.Mathematics;

namespace Hud.Widgets.SingleChild;
/// <summary>
/// Centers the child widget.
/// </summary>
public class Center : SingleChildWidget
{
    public Center(Widget child) : base(child) { }

    public override void Render(Context context)
    {
        var childSize = Child.GetSize();

        float x = context.Position.X + (context.Size.X / 2 - childSize.X / 2);
        float y = context.Position.Y - (context.Size.Y / 2 - childSize.Y / 2);

        Child.Render(new Context(context, new Vector2(x, y), childSize));
    }
}
