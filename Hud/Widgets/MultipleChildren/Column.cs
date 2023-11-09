using OpenTK.Mathematics;

namespace Hud.Widgets.MultipleChildren;

public class Column : MultipleChildrenWidget
{
    private readonly Alignment _alignment;

    private readonly float[] _sizes;

    public Column(Widget[] children, Alignment alignment = Alignment.Equal) : base(children)
    {
        _alignment = alignment;
    }

    public Column(Widget[] children, float[] sizes) : base(children)
    {
        if (children.Length != sizes.Length)
            throw new Exception("Sizes do not match");
        _alignment = Alignment.Predefined;
        _sizes = sizes;
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
            case Alignment.Predefined:
                RenderPredefined(context);
                break;
            case Alignment.Greedy:
                RenderGreedy(context);
                break;
            default:
                throw new NotImplementedException();
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

    private void RenderPredefined(Context context)
    {
        if (Children.Length != _sizes.Length)
            throw new Exception("Lengths do not match");

        float y = context.Position.Y;
        for (int i = 0; i < Children.Length; i++)
        {
            float height = context.Size.Y * _sizes[i];
            Children[i].Render(new Context(context, new Vector2(context.Position.X, y), new Vector2(context.Size.X, height)));
            y -= height;
        }
    }

    private void RenderGreedy(Context context)
    {
        float y = context.Position.Y;
        for (int i = 0; i < Children.Length; i++)
        {
            if (i < Children.Length - 1)
            {
                float height = Children[i].GetSize().Y;
                Children[i].Render(new Context(context, new Vector2(context.Position.X, y), new Vector2(context.Size.X, height)));
                y -= height;
            }
            else
            {
                float bottomPoint = context.Position.Y - context.Size.Y;
                Children[i].Render(new Context(context, new Vector2(context.Position.X, y), new Vector2(context.Size.X, y - bottomPoint)));
            }
        }
    }
}
