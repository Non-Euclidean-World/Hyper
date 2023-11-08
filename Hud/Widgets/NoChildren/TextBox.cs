using Hud.Widgets.Colors;
using OpenTK.Mathematics;

namespace Hud.Widgets.NoChildren;

public class TextBox : Widget
{
    public string Text;

    private readonly float _size;

    public Vector4 Color;

    public TextBox(string text, float size, Color color = Colors.Color.White)
    {
        Text = text;
        _size = size;
        Color = ColorGetter.GetVector(color);
    }

    public override Vector2 GetSize()
    {
        return Printer.GetTextSize(Text, _size);
    }

    public override void Render(Context context)
    {
        context.Shader.UseTexture(false);
        context.Shader.SetColor(Color);
        Printer.RenderStringTopLeft(context.Shader, Text, _size, context.Position.X, context.Position.Y);
    }
}
