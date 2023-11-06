using Hud.Widgets.Colors;
using OpenTK.Mathematics;

namespace Hud.Widgets.NoChildren;

public class TextBox : Widget
{
    public string Text;

    private readonly float _size;

    private readonly Vector4 _color;

    public TextBox(string text, float size, Color color = Color.White)
    {
        Text = text;
        _size = size;
        _color = ColorGetter.GetVector(color);
    }

    public override Vector2 GetSize()
    {
        return Printer.GetTextSize(Text, _size);
    }

    public override void Render(Context context)
    {
        context.Shader.UseTexture(false);
        context.Shader.SetColor(_color);
        Printer.RenderStringTopLeft(context.Shader, Text, _size, context.Position.X, context.Position.Y);
    }
}
