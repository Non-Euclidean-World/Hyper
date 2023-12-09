using Hud.Widgets.Colors;
using OpenTK.Mathematics;

namespace Hud.Widgets.NoChildren;

/// <summary>
/// Displays the given text.
/// </summary>
public class Text : Widget
{
    /// <summary>
    /// The content of the text.
    /// </summary>
    public string Content;

    private readonly float _size;

    /// <summary>
    /// The color of the text.
    /// </summary>
    public Vector4 Color;
    
    /// <summary>
    /// Creates an instance of Text class.
    /// </summary>
    /// <param name="text">The text to be displayed.</param>
    /// <param name="size">The size of text.</param>
    /// <param name="color">The color of text.</param>
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
