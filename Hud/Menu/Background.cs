using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hud.Menu.Colors;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Hud.Menu;
public class Background : IWidget
{
    private Color _color;

    private IWidget _child;

    public Background(Color color, IWidget child)
    {
        _color = color;
        _child = child;
    }

    public Vector2 GetSize()
    {
        return _child.GetSize();
    }

    public void Render(Context context)
    {
        GL.BindVertexArray(SharedVao.Instance.Vao);
        context.Shader.SetColor(ColorGetter.GetVector(_color));
        context.Shader.UseTexture(false);
        float x = context.Position.X + context.Size.X / 2;
        float y = context.Position.Y - context.Size.Y / 2;
        var model = Matrix4.CreateTranslation(x, y, 0);
        model = Matrix4.CreateScale(context.Size.X, context.Size.Y, 1) * model;
        context.Shader.SetModel(model);
        GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
        _child.Render(context);
    }
}
