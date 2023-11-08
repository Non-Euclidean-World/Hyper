using OpenTK.Mathematics;

namespace Hud.Widgets.MultipleChildren;
public class Row : MultipleChildrenWidget
{
    private readonly Alignment _alignment;

    private readonly float[] _sizes;
    
    public Row(Widget[] children, Alignment alignment = Alignment.Equal) : base(children)
    {
        _alignment = alignment;
    }

    public Row(Widget[] children, float[] sizes) : base(children)
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
            x += size.X;
            y = Math.Max(y, size.Y);
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

        float x = context.Position.X;
        foreach (var child in Children)
        {
            var childWidth = child.GetSize().X * ratio.X;
            child.Render(new Context(context, new Vector2(x, context.Position.Y), new Vector2(childWidth, context.Size.Y)));
            x += childWidth;
        }
    }

    private void RenderEqual(Context context)
    {
        float width = context.Size.X / Children.Length;
        for (int i = 0; i < Children.Length; i++)
        {
            float x = context.Position.X + i * width;
            Children[i].Render(new Context(context, new Vector2(x, context.Position.Y), new Vector2(width, context.Size.Y)));
        }
    }

    private void RenderPredefined(Context context)
    {
        if (Children.Length != _sizes.Length)
            throw new Exception("Lengths do not match");

        float x = context.Position.X;
        for (int i = 0; i < Children.Length; i++)
        {
            float width = context.Size.X * _sizes[i];
            Children[i].Render(new Context(context, new Vector2(x, context.Position.Y), new Vector2(width, context.Size.Y)));
            x += width;
        }
    }

    private void RenderGreedy(Context context)
    {
        float x = context.Position.X;
        for (int i = 0; i < Children.Length; i++)
        {
            if (i < Children.Length - 1)
            {
                float width = Children[i].GetSize().X;
                Children[i].Render(new Context(context, new Vector2(x, context.Position.Y), new Vector2(width, context.Size.Y)));
                x += width;
            }
            else
            {
                float rightmostPoint = context.Position.X + context.Size.X;
                Children[i].Render(new Context(context, new Vector2(x, context.Position.Y), new Vector2(rightmostPoint - x, context.Size.Y)));
            }
        }
    }
}
