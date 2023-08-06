using OpenTK.Mathematics;

namespace Hyper.HUD;

internal class HUDVertexBuilder
{
    private HUDVertex _vertex = new();

    public HUDVertexBuilder SetPosition(float x, float y)
    {
        _vertex.X = x;
        _vertex.Y = y;

        return this;
    }

    public HUDVertexBuilder SetTextureCoords(float u, float v)
    {
        _vertex.U = u;
        _vertex.V = v;

        return this;
    }

    public HUDVertex Build()
    {
        var result = _vertex;

        Reset();

        return result;
    }

    private void Reset()
    {
        _vertex = new HUDVertex();
    }
}