using System.Runtime.InteropServices;
using System.Text;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using SkiaSharp;

namespace Hyper.HUD;

internal static class Printer
{
    private static readonly Texture AsciiTexture;
    private static readonly int[] Vaos;
    
    private const int CharacterCount = 256;
    private const float CharacterHeight = 1.0f;
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
            canvas.Clear(SKColors.Transparent);
            float x = bitmap.Width / 2.0f;
            canvas.DrawText(text, x, FontSize - 5, paint);
        }

        AsciiTexture = Texture.LoadFromBitmap(bitmap);
        Vaos = GetVaos(text, paint);
    }

    public static void RenderString(Shader shader, string text, float size, float x, float y)
    {
        shader.SetBool("useTexture", true);
        float offset = size * 2;
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
        AsciiTexture.Use(TextureUnit.Texture0);
        GL.BindVertexArray(Vaos[c]);
        
        GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
    }
    
    private static int[] GetVaos(string text, SKPaint paint)
    {
        float textWidth = paint.MeasureText(text);

        var vaos = new int[CharacterCount];
        
        for (int i = 0; i < CharacterCount; i++)
        {
            float begin = paint.MeasureText(text[..i]) / textWidth;
            float end = paint.MeasureText(text[..(i + 1)]) / textWidth;
            
            HUDVertexBuilder builder = new();
            var vertices = new[]
            {
                builder.SetPosition(-1, -1).SetTextureCoords(begin, CharacterHeight).Build(),
                builder.SetPosition(1, -1).SetTextureCoords(end, CharacterHeight).Build(),
                builder.SetPosition(1, 1).SetTextureCoords(end, 0).Build(),

                builder.SetPosition(-1, -1).SetTextureCoords(begin, CharacterHeight).Build(),
                builder.SetPosition(1, 1).SetTextureCoords(end, 0).Build(),
                builder.SetPosition(-1, 1).SetTextureCoords(begin, 0).Build(),
            };

            int vao = GL.GenVertexArray();
            int vbo = GL.GenBuffer();

            GL.BindVertexArray(vao);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * Marshal.SizeOf<HUDVertex>(), vertices, BufferUsageHint.StaticDraw);


            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, Marshal.SizeOf<HUDVertex>(), 0);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, Marshal.SizeOf<HUDVertex>(), 2 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);

            vaos[i] = vao;
        }

        return vaos;
    }
}