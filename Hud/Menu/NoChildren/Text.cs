using Hud.Menu.Colors;
using OpenTK.Mathematics;

namespace Hud.Menu.NoChildren;

public class Text : Widget
{
    protected string _text;

    private readonly float _size;

    private readonly Color _color;

    public Text(string text, float size, Color color = Color.White)
    {
        _text = text;
        _size = size;
        _color = color;
    }

    public override Vector2 GetSize()
    {
        return Printer.GetTextSize(_text, _size);
    }

    public override void Render(Context context)
    {
        context.Shader.UseTexture(false);
        context.Shader.SetColor(ColorGetter.GetVector(_color));
        Printer.RenderStringTopLeft(context.Shader, _text, _size, context.Position.X, context.Position.Y);
    }
}
