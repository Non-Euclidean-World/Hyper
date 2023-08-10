using System.Text;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using SkiaSharp;

namespace Hyper.HUD;

internal static class Printer
{
    private static readonly Texture AsciiTexture;
    private static readonly Vector4[] Rectangles;
    
    private const int CharacterCount = 256;
    private const int FontSize = 50;
    
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

        using var bitmap = new SKBitmap((int)Math.Ceiling(textWidth), FontSize);
        using (SKCanvas canvas = new SKCanvas(bitmap))
        {
            canvas.Translate(0, bitmap.Height);
            canvas.Scale(1, -1);
            canvas.Clear(SKColors.Transparent);
            float x = bitmap.Width / 2.0f;
            canvas.DrawText(text, x, FontSize - 5, paint);
        }

        AsciiTexture = Texture.LoadFromBitmap(bitmap);
        Rectangles = GetRectangles(text, paint);
    }

    public static void RenderStringTopRight(Shader shader, string text, float size, float x, float y)
    {
        var centerX = x - (text.Length * size * 2 - size);
        var centerY = y - size;
        
        RenderString(shader, text, size, centerX, centerY);
    }
    
    public static void RenderStringBottomRight(Shader shader, string text, float size, float x, float y)
    {
        var centerX = x - (text.Length * size * 2 - size);
        var centerY = y + size;
        
        RenderString(shader, text, size, centerX, centerY);
    }

    public static void RenderString(Shader shader, string text, float size, float x, float y)
    {
        GL.BindVertexArray(SharedVao.Instance.Vao);
        shader.SetBool("useTexture", true);
        AsciiTexture.Use(TextureUnit.Texture0);

        float offset = GetOffset(size);
        for (int i = 0; i < text.Length; i++)
        {
            RenderChar(shader, text[i], size, x + i * offset, y);
        }
    }
    
    private static void RenderChar(Shader shader, char c, float size, float x, float y)
    {
        var model = Matrix4.CreateTranslation(x, y, 0.0f);
        model = Matrix4.CreateScale(size, size, 1.0f) * model;
        shader.SetMatrix4("model", model);
        shader.SetVector4("spriteRect", Rectangles[c]);
        
        GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
    }

    private static float GetOffset(float size) => 2 * size;
    
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