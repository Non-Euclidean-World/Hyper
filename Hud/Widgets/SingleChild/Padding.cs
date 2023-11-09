using OpenTK.Mathematics;

namespace Hud.Widgets.SingleChild;

public class Padding : SingleChildWidget
{
    private readonly float _size;

    public Padding(Widget child, float size) : base(child)
    {
        _size = size;
    }

    public override Vector2 GetSize()
    {
        var size = Child.GetSize();
        return new Vector2(size.X + _size * 2, size.Y + _size * 2);
    }

    public override void Render(Context context)
    {
        var position = context.Position + new Vector2(_size, -_size);
        var size = context.Size - new Vector2(2 * _size);
        Child.Render(new Context(context, position, size));
    }
}