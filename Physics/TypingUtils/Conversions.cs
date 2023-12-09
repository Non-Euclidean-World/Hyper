namespace Physics.TypingUtils;

/// <summary>
/// Provides methods for converting between different vector and matrix types from OpenTK and System.Numerics.
/// </summary>
public static class Conversions
{
    /// <summary>
    /// Converts an OpenTK Vector2 to System.Numerics Vector2.
    /// </summary>
    public static System.Numerics.Vector2 ToNumericsVector(OpenTK.Mathematics.Vector2 v)
        => new(v.X, v.Y);

    /// <summary>
    /// Converts an OpenTK Vector3 to System.Numerics Vector3.
    /// </summary>
    public static System.Numerics.Vector3 ToNumericsVector(OpenTK.Mathematics.Vector3 v)
        => new(v.X, v.Y, v.Z);

    /// <summary>
    /// Converts a System.Numerics Vector2 to OpenTK Vector2.
    /// </summary>
    public static OpenTK.Mathematics.Vector2 ToOpenTKVector(System.Numerics.Vector2 v)
        => new(v.X, v.Y);

    /// <summary>
    /// Converts a System.Numerics Vector3 to OpenTK Vector3.
    /// </summary>
    public static OpenTK.Mathematics.Vector3 ToOpenTKVector(System.Numerics.Vector3 v)
        => new(v.X, v.Y, v.Z);

    /// <summary>
    /// Converts a System.Numerics Matrix4x4 to OpenTK Matrix4.
    /// </summary>
    public static OpenTK.Mathematics.Matrix4 ToOpenTKMatrix(System.Numerics.Matrix4x4 m)
        => new(m.M11, m.M12, m.M13, m.M14,
            m.M21, m.M22, m.M23, m.M24,
            m.M31, m.M32, m.M33, m.M34,
            m.M41, m.M42, m.M43, m.M44);
}
