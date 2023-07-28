using System.Runtime.InteropServices;

namespace Hyper.HUD;

[StructLayout(LayoutKind.Explicit)]
internal struct HUDVertex
{
    [FieldOffset(0)]
    public float X;

    [FieldOffset(4)]
    public float Y;

    [FieldOffset(8)]
    public float R;

    [FieldOffset(12)]
    public float G;

    [FieldOffset(16)]
    public float B;

    [FieldOffset(20)]
    public float U;

    [FieldOffset(24)]
    public float V;


    public HUDVertex(float x, float y, float r, float g, float b, float u, float v)
    {
        X = x;
        Y = y;

        R = r;
        G = g;
        B = b;

        U = u;
        V = v;
    }
}