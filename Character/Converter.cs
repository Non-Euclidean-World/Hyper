namespace Character;

public static class Converter
{
    public static System.Numerics.Vector3 ToNumericsVector(Assimp.Vector3D v)
        => new(v.X, v.Y, v.Z);
    
    public static OpenTK.Mathematics.Vector3 ToOpenTKVector(Assimp.Vector3D v)
        => new(v.X, v.Y, v.Z);
    
    public static OpenTK.Mathematics.Matrix4 ToOpenTKMatrix(Assimp.Matrix4x4 m)
        => new(m.A1, m.A2, m.A3, m.A4,
            m.B1, m.B2, m.B3, m.B4,
            m.C1, m.C2, m.C3, m.C4,
            m.D1, m.D2, m.D3, m.D4);
}