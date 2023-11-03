using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace Hud.Menu;
public class Row : IWidget
{
    private IWidget[] _children;

    public Row(IWidget[] children)
    {
        _children = children;
    }

    public Vector2 GetSize()
    {
        float x = 0;
        float y = 0;

        foreach (var child in _children)
        {
            var size = child.GetSize();
            x += size.X;
            y = Math.Max(y, size.Y);
        }

        return new Vector2(x, y);
    }

    public void Render(Context context)
    {
        var size = GetSize();
        var ratio = size / context.Size;

        float x = context.Position.X;
        foreach (var child in _children)
        {
            var childWidth = child.GetSize().X * ratio.X;
            child.Render(new Context(context.Shader, new Vector2(x, context.Position.Y), new Vector2(childWidth, context.Size.Y)));
            x += childWidth;
        }
    }
}
