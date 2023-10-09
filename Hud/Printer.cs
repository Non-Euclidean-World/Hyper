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
    
    private static readonly float CharAspectRatio;

    static Printer()
    {
        var bytes = Enumerable.Range(0, CharacterCount).Select(i => (byte)i).ToArray();
        string text = Encoding.ASCII.GetString(bytes);

        using SKPaint paint = new SKPaint();
        paint.Color = SKColors.White;
        paint.IsAntialias = true;
        paint.TextSize = FontSize;
        paint.TextAlign = SKTextAlign.Center;
        paint.Typeface = SKTypeface.FromFamilyName("Courier");

        float textWidth = paint.MeasureText(text);
        paint.GetFontMetrics(out var metrics);

        float textHeight = -metrics.Ascent + metrics.Descent + metrics.Leading;
        
        using var bitmap = new SKBitmap((int)Math.Ceiling(textWidth), (int)Math.Ceiling(textHeight));
        using (SKCanvas canvas = new SKCanvas(bitmap))
        {
            canvas.Translate(0, bitmap.Height);
            canvas.Scale(1, -1);
            canvas.Clear(SKColors.Transparent);
            float x = bitmap.Width / 2.0f;
            canvas.DrawText(text, x, -metrics.Ascent, paint);
        }

        AsciiTexture = Texture.LoadFromBitmap(bitmap);
        Rectangles = GetRectangles(text, paint);
        CharAspectRatio = paint.MeasureText("a") / textHeight;
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
        var left = x - (text.Length * size * 2);
        var top = y;

        RenderStringTopLeft(shader, text, size, left, top);
    }

    public static void RenderStringTopLeft(Shader shader, string text, float size, float x, float y)
    {
        var left = x + size;
        var top = y - size * 1.5f;

        GL.BindVertexArray(SharedVao.Instance.Vao);
        shader.SetBool("useTexture", true);
        AsciiTexture.Use(TextureUnit.Texture0);

        float verticalOffset = 0;
        float horizontalOffset = 0;
        foreach (var t in text)
        {
            if (t == '\n')
            {
                verticalOffset += 3 * size * 1.1f;
                horizontalOffset = 0;
                continue;
            }
            RenderChar(shader, t, size, left + horizontalOffset, top - verticalOffset);
            horizontalOffset += 2 * size;
        }
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
        var left = x - (text.Length * size * 2);
        var top = y + 2 * size * 1.5f;

        RenderStringTopLeft(shader, text, size, left, top);
    }

    private static void RenderChar(Shader shader, char c, float size, float x, float y)
    {
        var model = Matrix4.CreateTranslation(x, y, 0.0f);
        model = Matrix4.CreateScale(size, size / CharAspectRatio, 1.0f) * model;
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