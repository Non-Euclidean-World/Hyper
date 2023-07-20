using System.Runtime.InteropServices;

namespace Hyper.Meshes;

[StructLayout(LayoutKind.Explicit)]
internal struct Vertex
{
    [FieldOffset(0)]
    public float X;
    
    [FieldOffset(4)]
    public float Y;
    
    [FieldOffset(8)]
    public float Z;

    [FieldOffset(12)] 
    public float Nx;
    
    [FieldOffset(16)]
    public float Ny;
    
    [FieldOffset(20)]
    public float Nz;
    
    public Vertex(float x, float y, float z, float nx, float ny, float nz)
    {
        X = x;
        Y = y;
        Z = z;
        Nx = nx;
        Ny = ny;
        Nz = nz;
    }
}