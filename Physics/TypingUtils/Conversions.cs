namespace Physics.TypingUtils;
public static class Conversions
{
    public static System.Numerics.Vector2 ToNumericsVector(OpenTK.Mathematics.Vector2 v)
        => new(v.X, v.Y);

    public static System.Numerics.Vector3 ToNumericsVector(OpenTK.Mathematics.Vector3 v)
        => new(v.X, v.Y, v.Z);

    public static OpenTK.Mathematics.Vector2 ToOpenTKVector(System.Numerics.Vector2 v)
        => new(v.X, v.Y);

    public static OpenTK.Mathematics.Vector3 ToOpenTKVector(System.Numerics.Vector3 v)
        => new(v.X, v.Y, v.Z);

    public static OpenTK.Mathematics.Vector4 ToOpenTKVector(System.Numerics.Vector4 v)
        => new(v.X, v.Y, v.Z, v.W);

    public static OpenTK.Mathematics.Matrix4 ToOpenTKMatrix(System.Numerics.Matrix4x4 m)
        => new(m.M11, m.M12, m.M13, m.M14,
            m.M21, m.M22, m.M23, m.M24,
            m.M31, m.M32, m.M33, m.M34,
            m.M41, m.M42, m.M43, m.M44);
}
