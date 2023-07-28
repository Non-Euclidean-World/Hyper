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

    public HUDVertexBuilder SetColor(float r, float g, float b)
    {
        _vertex.R = r;
        _vertex.G = g;
        _vertex.B = b;

        return this;
    }

    public HUDVertexBuilder SetColor(Vector3 color)
    {
        _vertex.R = color.X;
        _vertex.G = color.Y;
        _vertex.B = color.Z;

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