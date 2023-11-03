using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hud.Menu.Colors;
using OpenTK.Mathematics;

namespace Hud.Menu;

public class Text : IWidget
{
    private readonly string _text;

    private readonly float _size;

    private readonly Color _color;

    public Text(string text, float size, Color color = Color.White)
    {
        _text = text;
        _size = size;
        _color = color;
    }

    public Vector2 GetSize()
    {
        return Printer.GetTextSize(_text, _size);
    }

    public void Render(Context context)
    {
        context.Shader.UseTexture(false);
        context.Shader.SetColor(ColorGetter.GetVector(_color));
        Printer.RenderStringTopLeft(context.Shader, _text, _size, context.Position.X, context.Position.Y);
    }
}
