using OpenTK.Mathematics;

namespace Hud.Widgets.MultipleChildren;

public class Column : MultipleChildrenWidget
{
    private readonly Alignment _alignment;

    public Column(Widget[] children, Alignment alignment = Alignment.Equal) : base(children)
    {
        _alignment = alignment;
    }

    public override Vector2 GetSize()
    {
        float x = 0;
        float y = 0;

        foreach (var child in Children)
        {
            var size = child.GetSize();
            x = Math.Max(x, size.X);
            y += size.Y;
        }

        return new Vector2(x, y);
    }

    public override void Render(Context context)
    {
        switch (_alignment)
        {
            case Alignment.Proportional:
                RenderProportional(context);
                break;
            case Alignment.Equal:
                RenderEqual(context);
                break;
            default:
                throw new NotImplementedException();
                break;
        }
    }

    private void RenderProportional(Context context)
    {
        var size = GetSize();
        var ratio = context.Size / size;

        float y = context.Position.Y;
        foreach (var child in Children)
        {
            var childHeight = child.GetSize().Y * ratio.Y;
            child.Render(new Context(context, new Vector2(context.Position.X, y), new Vector2(context.Size.X, childHeight)));
            y -= childHeight;
        }
    }

    private void RenderEqual(Context context)
    {
        float height = context.Size.Y / Children.Length;
        for (int i = 0; i < Children.Length; i++)
        {
            float y = context.Position.Y - i * height;
            Children[i].Render(new Context(context, new Vector2(context.Position.X, y), new Vector2(context.Size.X, height)));
        }
    }
}
