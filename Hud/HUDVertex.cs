using System.Runtime.InteropServices;

namespace Hud;

[StructLayout(LayoutKind.Explicit)]
internal struct HUDVertex
{
    [FieldOffset(0)]
    public float X;

    [FieldOffset(4)]
    public float Y;

    [FieldOffset(8)]
    public float U;

    [FieldOffset(12)]
    public float V;


    public HUDVertex(float x, float y, float u, float v)
    {
        X = x;
        Y = y;

        U = u;
        V = v;
    }
}