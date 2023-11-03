using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace Hud.Menu;
internal class Text : IWidget
{
    private readonly string _text;

    private readonly float _size;

    public Vector2 GetSize()
    {
        return Printer.GetTextSize(_text, _size);
    }

    public void Render(Context context)
    {
        Printer.RenderStringTopLeft(context.Shader, _text, _size, context.Position.X, context.Position.Y);
    }
}
