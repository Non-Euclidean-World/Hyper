using Hud.Widgets.Colors;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Hud.Widgets.SingleChild;
public class Background : SingleChildWidget
{
    private readonly Vector4 _color;

    public Background(Widget child, Color color = Color.Background) : base(child)
    {
        _color = ColorGetter.GetVector(color);
    }

    public Background(Widget child, Vector4 color) : base(child)
    {
        _color = color;
    }

    public override void Render(Context context)
    {
        GL.BindVertexArray(SharedVao.Instance.Vao);
        context.Shader.SetColor(_color);
        context.Shader.UseTexture(false);
        float x = context.Position.X + context.Size.X / 2;
        float y = context.Position.Y - context.Size.Y / 2;
        var model = Matrix4.CreateTranslation(x, y, 0);
        model = Matrix4.CreateScale(0.5f * context.Size.X, 0.5f * context.Size.Y, 1) * model; // 0.5f * is because SharedVao.Instance.Vao has size 2.
        context.Shader.SetModel(model);
        GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
        Child.Render(context);
    }
}
