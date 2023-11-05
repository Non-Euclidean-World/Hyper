using Hud.Menu.Colors;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Hud.Menu.SingleChild;
public class Background : SingleChildWidget
{
    private readonly Color _color;

    public Background(Color color, Widget child) : base(child)
    {
        _color = color;
    }

    public override Vector2 GetSize()
    {
        return Child.GetSize();
    }

    public override void Render(Context context)
    {
        GL.BindVertexArray(SharedVao.Instance.Vao);
        context.Shader.SetColor(ColorGetter.GetVector(_color));
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
