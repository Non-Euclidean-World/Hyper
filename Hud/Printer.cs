using System.Text;
using Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using SkiaSharp;

namespace Hud;

public static class Printer
{
    private static readonly Texture AsciiTexture;
    private static readonly Vector4[] Rectangles;

    private const int CharacterCount = 256;
    private const int FontSize = 50;

    private static readonly SKPaint Paint;
    private static readonly float CharHeight;

    static Printer()
    {
        var bytes = Enumerable.Range(0, CharacterCount).Select(i => (byte)i).ToArray();
        string text = Encoding.ASCII.GetString(bytes);

        Paint = new SKPaint();
        Paint.Color = SKColors.White;
        Paint.IsAntialias = true;
        Paint.TextSize = FontSize;
        Paint.TextAlign = SKTextAlign.Center;
        Paint.Typeface = SKTypeface.FromFamilyName("Courier");

        float textWidth = Paint.MeasureText(text);
        Paint.GetFontMetrics(out var metrics);

        CharHeight = -metrics.Ascent + metrics.Descent + metrics.Leading;

        using var bitmap = new SKBitmap((int)Math.Ceiling(textWidth), (int)Math.Ceiling(CharHeight));
        using (SKCanvas canvas = new SKCanvas(bitmap))
        {
            canvas.Translate(0, bitmap.Height);
            canvas.Scale(1, -1);
            canvas.Clear(SKColors.Transparent);
            float x = bitmap.Width / 2.0f;
            canvas.DrawText(text, x, -metrics.Ascent, Paint);
        }

        AsciiTexture = Texture.LoadFromBitmap(bitmap);
        Rectangles = GetRectangles(text, Paint);
    }

    /// <summary>
    /// Renders the text with the top left corner at the given position.
    /// </summary>
    /// <param name="shader"></param>
    /// <param name="text"></param>
    /// <param name="size"></param>
    /// <param name="top"></param>
    /// <param name="left"></param>
    public static void RenderStringTopLeft(Shader shader, string text, float size, float left, float top)
    {
        var x = left;
        var y = top - size / 2;

        GL.BindVertexArray(SharedVao.Instance.Vao);
        shader.SetBool("useTexture", true);
        AsciiTexture.Use(TextureUnit.Texture0);

        float verticalOffset = 0;
        float horizontalOffset = 0;
        foreach (var t in text)
        {
            if (t == '\n')
            {
                verticalOffset += size * 1.2f;
                horizontalOffset = 0;
                continue;
            }

            var charOffset = size * Paint.MeasureText(t.ToString()) / CharHeight;
            RenderChar(shader, t, size, x + horizontalOffset + charOffset, y - verticalOffset);
            horizontalOffset += 2 * size * Paint.MeasureText(t.ToString()) / CharHeight;
        }
    }

    /// <summary>
    /// Renders the text with the top right corner at the given position.
    /// </summary>
    /// <param name="shader"></param>
    /// <param name="text"></param>
    /// <param name="size"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public static void RenderStringTopRight(Shader shader, string text, float size, float x, float y)
    {
        var left = x - 2 * size * Paint.MeasureText(text) / CharHeight;
        var top = y;

        RenderStringTopLeft(shader, text, size, left, top);
    }

    /// <summary>
    /// Renders the text with the bottom right corner at the given position.
    /// </summary>
    /// <param name="shader"></param>
    /// <param name="text"></param>
    /// <param name="size"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public static void RenderStringBottomRight(Shader shader, string text, float size, float x, float y)
    {
        var left = x - 2 * size * Paint.MeasureText(text) / CharHeight;
        var top = y + size;

        RenderStringTopLeft(shader, text, size, left, top);
    }

    /// <summary>
    /// Renders char with the middle at the given position.
    /// </summary>
    /// <param name="shader"></param>
    /// <param name="c"></param>
    /// <param name="size"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    private static void RenderChar(Shader shader, char c, float size, float x, float y)
    {
        var model = Matrix4.CreateTranslation(x, y, 0.0f);
        model = Matrix4.CreateScale(size * Paint.MeasureText(c.ToString()) / CharHeight, size, 1.0f) * model;
        shader.SetMatrix4("model", model);
        shader.SetVector4("spriteRect", Rectangles[c]);

        GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
    }

    private static Vector4[] GetRectangles(string text, SKPaint paint)
    {
        float textWidth = paint.MeasureText(text);

        var rectangles = new Vector4[CharacterCount];

        for (int i = 0; i < CharacterCount; i++)
        {
            float begin = paint.MeasureText(text[..i]) / textWidth;
            float end = paint.MeasureText(text[..(i + 1)]) / textWidth;

            rectangles[i] = new Vector4(begin, 0, end - begin, 1);
        }

        return rectangles;
    }
}