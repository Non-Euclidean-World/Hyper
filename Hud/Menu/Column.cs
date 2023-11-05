using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using static OpenTK.Graphics.OpenGL.GL;

namespace Hud.Menu;

public class Column : MultipleChildWidget
{
    private ColumnAllignment _allignment;

    public Column(Widget[] children, ColumnAllignment allignment = ColumnAllignment.Proportional) : base(children)
    {
        _allignment = allignment;
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
        switch (_allignment)
        {
            case ColumnAllignment.Proportional:
                RenderProportional(context);
                break;
            case ColumnAllignment.Equal:
                RenderEqual(context);
                break;
            default:
                RenderProportional(context);
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
        var size = GetSize();

        float height = Children.Length * size.Y;
        for (int i = 0; i < Children.Length; i++)
        {
            float y = i / Children.Length * size.Y;
            Children[i].Render(new Context(context, new Vector2(context.Position.X, y), new Vector2(context.Size.X, height)));
        }
    }
    
    
}

public enum ColumnAllignment
{
    Proportional,
    Equal
}
