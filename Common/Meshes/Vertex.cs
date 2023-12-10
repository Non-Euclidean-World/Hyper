using System.Runtime.InteropServices;
using OpenTK.Mathematics;

namespace Common.Meshes;

/// <summary>
/// Represents a vertex in a 3D environment, defining position, normal, and color attributes.
/// </summary>
[StructLayout(LayoutKind.Explicit)]
public struct Vertex
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

    /// <summary>
    /// Initializes a new instance of the <see cref="Vertex"/> struct with specified position and normal.
    /// </summary>
    /// <param name="x">The X-coordinate of the vertex position.</param>
    /// <param name="y">The Y-coordinate of the vertex position.</param>
    /// <param name="z">The Z-coordinate of the vertex position.</param>
    /// <param name="nx">The X-component of the vertex normal.</param>
    /// <param name="ny">The Y-component of the vertex normal.</param>
    /// <param name="nz">The Z-component of the vertex normal.</param>
    public Vertex(float x, float y, float z, float nx, float ny, float nz)
    {
        X = x;
        Y = y;
        Z = z;

        Nx = nx;
        Ny = ny;
        Nz = nz;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Vertex"/> struct with specified position, normal, and color.
    /// </summary>
    /// <param name="position">The position of the vertex.</param>
    /// <param name="normal">The normal vector of the vertex.</param>
    /// <param name="color">The color of the vertex.</param>
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

    /// <summary>
    /// Gets the position of the vertex as a <see cref="Vector3"/>.
    /// </summary>
    public readonly Vector3 Position => new Vector3(X, Y, Z);
}