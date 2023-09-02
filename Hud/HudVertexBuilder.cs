namespace Hud;

internal class HudVertexBuilder
{
    private HudVertex _vertex = new();

    public HudVertexBuilder SetPosition(float x, float y)
    {
        _vertex.X = x;
        _vertex.Y = y;

        return this;
    }

    public HudVertexBuilder SetTextureCoords(float u, float v)
    {
        _vertex.U = u;
        _vertex.V = v;

        return this;
    }

    public HudVertex Build()
    {
        var result = _vertex;

        Reset();

        return result;
    }

    private void Reset()
    {
        _vertex = new HudVertex();
    }
}