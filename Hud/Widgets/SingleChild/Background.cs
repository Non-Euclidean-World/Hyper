using Hud.Widgets.Colors;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Hud.Widgets.SingleChild;
/// <summary>
/// Sets the background color.
/// </summary>
public class Background : SingleChildWidget
{
    public Vector4 Color;

    /// <summary>
    /// Creates an instance of Background class.
    /// </summary>
    /// <param name="child">The widget inside the background.</param>
    /// <param name="color">The background color.</param>
    public Background(Widget child, Color color = Colors.Color.Background) : base(child)
    {
        Color = ColorGetter.GetVector(color);
    }

    public override void Render(Context context)
    {
        GL.BindVertexArray(SharedVao.Instance.Vao);
        context.Shader.SetColor(Color);
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
