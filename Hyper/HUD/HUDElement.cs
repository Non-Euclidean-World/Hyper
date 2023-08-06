using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Hyper.HUD;

internal abstract class HudElement
{
    public bool Visible { get; set; } = true;

    protected readonly int VaoId;

    protected Vector2 Position;

    protected readonly float Size;

    protected HudElement(Vector2 position, float size, HUDVertex[] vertices)
    {
        Position = position;
        Size = size;

        int vao = GL.GenVertexArray();
        GL.BindVertexArray(vao);

        int vbo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * Marshal.SizeOf<HUDVertex>(), vertices, BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, Marshal.SizeOf<HUDVertex>(), 0);
        GL.EnableVertexAttribArray(0);

        GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, Marshal.SizeOf<HUDVertex>(), 2 * sizeof(float));
        GL.EnableVertexAttribArray(1);

        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.BindVertexArray(0);

        VaoId = vao;
    }

    protected HudElement(Vector2 position, float size)
    {
        Position = position;
        Size = size;
    }

    public abstract void Render(Shader shader);
}
