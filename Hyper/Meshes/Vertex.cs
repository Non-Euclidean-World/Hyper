using System.Runtime.InteropServices;

namespace Hyper.Meshes;

[StructLayout(LayoutKind.Explicit)]
internal struct Vertex
{
    [FieldOffset(0)]
    internal float X;
    
    [FieldOffset(4)]
    internal float Y;
    
    [FieldOffset(8)]
    internal float Z;

    [FieldOffset(12)] 
    internal float Nx;
    
    [FieldOffset(16)]
    internal float Ny;
    
    [FieldOffset(20)]
    internal float Nz;
    
    internal Vertex(float x, float y, float z, float nx, float ny, float nz)
    {
        X = x;
        Y = y;
        Z = z;
        Nx = nx;
        Ny = ny;
        Nz = nz;
    }
}