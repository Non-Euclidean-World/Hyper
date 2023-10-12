using Common;
using OpenTK.Mathematics;

namespace Hud.HUDElements;
public class TipText : IHudElement
{
    public bool Visible { get => _shouldDisplayTip(); set => throw new NotImplementedException(); }

    private readonly float _size = 0.03f;

    private readonly string _text;

    private readonly Func<bool> _shouldDisplayTip;

    private readonly float _left;

    private readonly float _top;

    public TipText(string text, Func<bool> shouldDisplayTip, float top = 0, float left = 0)
    {
        _text = text;
        _shouldDisplayTip = shouldDisplayTip;
        _left = left;
        _top = top;
    }

    public void Render(Shader shader)
    {
        shader.SetVector4("color", Vector4.One);

        Printer.RenderStringTopLeft(shader, _text, _size, _left, _top);
    }

    public void Dispose()
    {
        // nothing to dispose
    }
}
