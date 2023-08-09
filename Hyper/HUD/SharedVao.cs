using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;

namespace Hyper.HUD;

public class SharedVao
{
    private static SharedVao? _instance;
    
    public static SharedVao Instance => _instance ??= new SharedVao();

    public readonly int Vao;
    
    private SharedVao()
    {
        Vao = GetVao();
    }

    private static int GetVao()
    {
        HUDVertexBuilder builder = new();
        var vertices = new[]
        {
            builder.SetPosition(-1, -1).SetTextureCoords(0, 0).Build(),
            builder.SetPosition(1, -1).SetTextureCoords(1, 0).Build(),
            builder.SetPosition(1, 1).SetTextureCoords(1, 1).Build(),

            builder.SetPosition(-1, -1).SetTextureCoords(0, 0).Build(),
            builder.SetPosition(1, 1).SetTextureCoords(1, 1).Build(),
            builder.SetPosition(-1, 1).SetTextureCoords(0, 1).Build(),
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

        return vao;
    }
}