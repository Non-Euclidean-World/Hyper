using System.Runtime.InteropServices;
using OpenTK.Mathematics;

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

    [FieldOffset(24)]
    public float R = 1f;

    [FieldOffset(28)]
    public float G = 1f;

    [FieldOffset(32)]
    public float B = 1f;

    public Vertex(float x, float y, float z, float nx, float ny, float nz)
    {
        X = x;
        Y = y;
        Z = z;

        Nx = nx;
        Ny = ny;
        Nz = nz;
    }

    public Vertex(Vector3 position, Vector3 normal, Vector3 color)
    {
        X = position.X;
        Y = position.Y;
        Z = position.Z;

        Nx = normal.X;
        Ny = normal.Y;
        Nz = normal.Z;

        R = color.X;
        G = color.Y;
        B = color.Z;
    }
}