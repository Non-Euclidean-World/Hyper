using System.Runtime.InteropServices;

namespace Hyper.HUD;

[StructLayout(LayoutKind.Explicit)]
internal struct Vertex2d
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
    public float Tx;
    
    [FieldOffset(24)]
    public float Ty;
    
    
    public Vertex2d(float x, float y, float r, float g, float b, float tx, float ty)
    {
        X = x;
        Y = y;
        R = r;
        G = g;
        B = b;
        Tx = tx;
        Ty = ty;
    }
}