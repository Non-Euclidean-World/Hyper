using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;

namespace Hud;

public class SharedVao
{
    private static readonly Lazy<SharedVao> InternalInstance = new(() => new SharedVao());

    public static SharedVao Instance => InternalInstance.Value;

    public readonly int Vao;

    private SharedVao()
    {
        Vao = GetVao();
    }

    private static int GetVao()
    {
        HudVertexBuilder builder = new();
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
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * Marshal.SizeOf<HudVertex>(), vertices, BufferUsageHint.StaticDraw);


        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, Marshal.SizeOf<HudVertex>(), 0);
        GL.EnableVertexAttribArray(0);

        GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, Marshal.SizeOf<HudVertex>(), 2 * sizeof(float));
        GL.EnableVertexAttribArray(1);

        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);

        return vao;
    }
}