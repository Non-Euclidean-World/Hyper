using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace Hud.Menu;
internal class Column : IWidget
{
    private IWidget[] _children;

    public Column(IWidget[] children)
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
            x = Math.Max(x, size.X);
            y += size.Y;
        }

        return new Vector2(x, y);
    }

    public void Render(Context context)
    {
        var size = GetSize();
        var ratio = size / context.Size;

        float y = context.Position.Y;
        foreach (var child in _children)
        {
            var childHeight = child.GetSize().Y * ratio.Y;
            child.Render(new Context(context.Shader, new Vector2(context.Position.X, y), new Vector2(context.Size.X, childHeight)));
            y += childHeight;
        }
    }
}
