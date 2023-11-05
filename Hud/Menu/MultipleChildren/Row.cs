using OpenTK.Mathematics;

namespace Hud.Menu.MultipleChildren;
public class Row : MultipleChildWidget
{
    public Row(Widget[] children) : base(children)
    {
    }

    public override Vector2 GetSize()
    {
        float x = 0;
        float y = 0;

        foreach (var child in Children)
        {
            var size = child.GetSize();
            x += size.X;
            y = Math.Max(y, size.Y);
        }

        return new Vector2(x, y);
    }

    public override void Render(Context context)
    {
        var size = GetSize();
        var ratio = context.Size / size;

        float x = context.Position.X;
        foreach (var child in Children)
        {
            var childWidth = child.GetSize().X * ratio.X;
            child.Render(new Context(context, new Vector2(x, context.Position.Y), new Vector2(childWidth, context.Size.Y)));
            x += childWidth;
        }
    }
}
