using System.Runtime.InteropServices;

namespace Hud;

[StructLayout(LayoutKind.Explicit)]
internal struct HudVertex
{
    [FieldOffset(0)]
    public float X;

    [FieldOffset(4)]
    public float Y;

    [FieldOffset(8)]
    public float U;

    [FieldOffset(12)]
    public float V;


    public HudVertex(float x, float y, float u, float v)
    {
        X = x;
        Y = y;

        U = u;
        V = v;
    }
}