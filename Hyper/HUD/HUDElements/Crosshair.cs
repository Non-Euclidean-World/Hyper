using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Hyper.HUD.HUDElements;

internal class Crosshair : HudElement
{
    public const float DefaultSize = 0.02f;

    private static readonly HUDVertex[] Vertices = InitializeVertices();

    public Crosshair(Vector2 position, float size) : base(position, size, Vertices) { }

    public override void Render(Shader shader)
    {
        var model = Matrix4.CreateTranslation(Position.X, Position.Y, 0.0f);
        model *= Matrix4.CreateScale(Size, Size, 1.0f);

        shader.SetMatrix4("model", model);
        shader.SetBool("useTexture", false);
        shader.SetVector4("color", new Vector4(1, 0, 0, 1));
        GL.BindVertexArray(VaoId);
        GL.DrawArrays(PrimitiveType.Lines, 0, 4);
    }

    public static HUDVertex[] InitializeVertices()
    {
        HUDVertexBuilder builder = new();
        Vector3 color = new Vector3(1, 0, 0);
        return new[]
        {
            builder.SetPosition(-1, 0).Build(),
            builder.SetPosition(1, 0).Build(),
            builder.SetPosition(0, 1).Build(),
            builder.SetPosition(0, -1).Build()
        };
    }
}
