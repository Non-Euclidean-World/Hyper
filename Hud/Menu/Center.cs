using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace Hud.Menu;
public class Center : IWidget
{
    private IWidget _child;

    public Center(IWidget child) { _child = child; }

    public Vector2 GetSize()
    {
        return _child.GetSize();
    }

    public void Render(Context context)
    {
        var childSize = _child.GetSize();

        float x = context.Position.X + context.Size.X / 2;
        float y = context.Position.Y - context.Size.Y / 2;

        _child.Render(new Context(context.Shader, new Vector2(x, y), childSize));
    }
}
