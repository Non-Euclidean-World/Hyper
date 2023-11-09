using OpenTK.Mathematics;

namespace Hud.Widgets.SingleChild;

public class SizeBox : SingleChildWidget
{
    private readonly Vector2 _size;

    public SizeBox(Widget child, Vector2 size) : base(child)
    {
        _size = size;
    }

    public override Vector2 GetSize()
    {
        return _size;
    }

    public override void Render(Context context)
    {
        Child.Render(new Context(context, context.Position, _size));
    }
}