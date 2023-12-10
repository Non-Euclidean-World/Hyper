using OpenTK.Mathematics;

namespace Common;

/// <summary>
/// Provides methods for porting points from Euclidean to non-Euclidean spaces, reflecting vectors, and calculating generalized dot products.
/// </summary>
public static class GeomPorting
{
    /// <summary>
    /// Represents the geometry origin.
    /// </summary>
    public static readonly Vector4 GeometryOrigin = Vector4.UnitW;

    /// <summary>
    /// Ports a point in Euclidean space to non-Euclidean space.
    /// </summary>
    /// <param name="eucPoint"></param>
    /// <param name="curve">If curve is equal 0 we get the matrix in Euclidean space. If it's less than 0 in hyperbolic space and if greater than 0 in spherical.</param>
    /// <returns>The point in the specified non-Euclidean space.</returns>
    public static Vector4 EucToCurved(Vector3 eucPoint, float curve)
    {
        return EucToCurved(new Vector4(eucPoint, 1), curve);
    }

    /// <summary>
    /// Ports a point in non-Euclidean space to Euclidean space.
    /// </summary>
    /// <param name="eucPoint"></param>
    /// <param name="curve">If curve is equal 0 we get the matrix in Euclidean space. If it's less than 0 in hyperbolic space and if greater than 0 in spherical.</param>
    /// <returns>The point in the specified non-Euclidean space.</returns>
    public static Vector4 EucToCurved(Vector4 eucPoint, float curve)
    {
        Vector3 p = eucPoint.Xyz;
        float dist = p.Length;
        if (dist < 0.0001f) return eucPoint;
        if (curve > 0) return new Vector4(p / dist * (float)MathHelper.Sin(dist), (float)MathHelper.Cos(dist));
        if (curve < 0) return new Vector4(p / dist * (float)MathHelper.Sinh(dist), (float)MathHelper.Cosh(dist));
        return eucPoint;
    }

    /// <summary>
    /// Creates translation target valid in all geometries.
    /// Since in the hyperbolic geometry the camera is fixed, any request to change camera position must be reflected in changing the object's position.
    /// </summary>
    /// <param name="to">World-space coordinates of the translation target</param>
    /// <param name="referencePoint">Reference point of the camera</param>
    /// <param name="curve">Geometry curvature</param>
    /// <returns>The translation target position.</returns>
    public static Vector3 CreateTranslationTarget(Vector3 to, Vector3 referencePoint, float curve, float scale)
    {
        if (curve >= 0)
            return to * scale;

        return (to - referencePoint) * scale;
    }

    /// <summary>
    /// Reflects a vector through a plane.
    /// </summary>
    /// <param name="v">Vector to be reflected</param>
    /// <param name="surfaceNormal">Normal vector of the reflection plane</param>
    /// <returns>The reflected vector.</returns>
    public static Vector3 ReflectVector(Vector3 v, Vector3 surfaceNormal)
    {
        return v - 2 * Vector3.Dot(v, surfaceNormal) * surfaceNormal;
    }

    /// <summary>
    /// Calculates the generalized dot product
    /// </summary>
    /// <param name="u"></param>
    /// <param name="v"></param>
    /// <param name="curve">Curvature</param>
    /// <returns>The result of the dot product calculation.</returns>
    public static float DotProduct(Vector4 u, Vector4 v, float curve)
    {
        if (curve < 0)
            return Vector4.Dot(u, v) - 2 * u.W * v.W;

        return Vector4.Dot(u, v);
    }
}
