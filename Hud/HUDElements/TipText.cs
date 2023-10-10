using Common;
using OpenTK.Mathematics;

namespace Hud.HUDElements;
public class TipText : IHudElement
{
    public bool Visible { get; set; } = true;

    private readonly float _size = 0.01f;

    public void Dispose()
    {
        // nothing to dispose
    }

    public void Render(Shader shader)
    {
        shader.SetVector4("color", Vector4.One);

        Printer.RenderString(shader, "C to enter the car", _size, 0, 0);
    }
}
