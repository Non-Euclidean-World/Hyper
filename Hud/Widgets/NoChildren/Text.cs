using Hud.Widgets.Colors;
using OpenTK.Mathematics;

namespace Hud.Widgets.NoChildren;

public class Text : Widget
{
    public string Content;

    private readonly float _size;

    public Vector4 Color;

    public Text(string text, float size, Color color = Colors.Color.White)
    {
        Content = text;
        _size = size;
        Color = ColorGetter.GetVector(color);
    }

    public override Vector2 GetSize()
    {
        return Printer.GetTextSize(Content, _size);
    }

    public override void Render(Context context)
    {
        context.Shader.UseTexture(false);
        context.Shader.SetColor(Color);
        Printer.RenderStringTopLeft(context.Shader, Content, _size, context.Position.X, context.Position.Y);
    }
}
