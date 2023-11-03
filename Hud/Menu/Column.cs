using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using static OpenTK.Graphics.OpenGL.GL;

namespace Hud.Menu;

public class Column : IWidget
{
    private IWidget[] _children;

    private ColumnAllignment _allignment;

    public Column(IWidget[] children, ColumnAllignment allignment = ColumnAllignment.Proportional)
    {
        _children = children;
        _allignment = allignment;
    }

    public Vector2 GetSize()
    {
        float x = 0;
        float y = 0;

        foreach (var child in _children)
        {
            var size = child.GetSize();
            x = Math.Max(x, size.X);
            y += size.Y;
        }

        return new Vector2(x, y);
    }

    public void Render(Context context)
    {
        switch (_allignment)
        {
            case ColumnAllignment.Proportional:
                RenderProportional(context);
                break;
            case ColumnAllignment.Equal:

                break;
            default:
                RenderProportional(context);
                break;
        }
        
    }

    public void RenderProportional(Context context)
    {
        var size = GetSize();
        var ratio = context.Size / size;

        float y = context.Position.Y;
        foreach (var child in _children)
        {
            var childHeight = child.GetSize().Y * ratio.Y;
            child.Render(new Context(context.Shader, new Vector2(context.Position.X, y), new Vector2(context.Size.X, childHeight)));
            y -= childHeight;
        }
    }

    public void RenderEqual(Context context)
    {
        var size = GetSize();

        float height = _children.Length * size.Y;
        for (int i = 0; i < _children.Length; i++)
        {
            float y = i / _children.Length * size.Y;
            _children[i].Render(new Context(context.Shader, new Vector2(context.Position.X, y), new Vector2(context.Size.X, height)));
        }
    }
}

public enum ColumnAllignment
{
    Proportional,
    Equal
}
