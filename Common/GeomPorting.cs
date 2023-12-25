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
    /// <param name="eucPoint">Point in Euclidean space.</param>
    /// <param name="curve">If curve is equal 0 we get the matrix in Euclidean space. If it's less than 0 in hyperbolic space and if greater than 0 in spherical.</param>
    /// <returns>The point in the specified non-Euclidean space.</returns>
    public static Vector4 EucToCurved(Vector3 eucPoint, float curve, int sphere, Vector3 sphereCenter)
    {
        return EucToCurved(new Vector4(eucPoint, 1), curve, sphere, sphereCenter);
    }

    /// <summary>
    /// Ports a point in non-Euclidean space to Euclidean space.
    /// </summary>
    /// <param name="eucPoint">Point in Euclidean space.</param>
    /// <param name="curve">If curve is equal 0 we get the matrix in Euclidean space. If it's less than 0 in hyperbolic space and if greater than 0 in spherical.</param>
    /// <returns>The point in the specified non-Euclidean space.</returns>
    public static Vector4 EucToCurved(Vector4 eucPoint, float curve, int sphere, Vector3 sphereCenter)
    {
        Vector3 p;
        if (sphere == 0)
        {
            p = eucPoint.Xyz;
        }
        else
        {
            // we're not shifting here because it's only used for the view matrix and the camera position is already shifted by the transporter
            // TODO this is hellishly error-prone but I can't get it to work, too many things rely on the shift in spherical transporter. This needs to be sorted out some day.
            p = eucPoint.Xyz /*- sphereCenter*/;
        }

        float dist = p.Length;
        if (dist < 0.0001f) return eucPoint;
        if (curve > 0)
        {
            if (sphere == 0)
                return new Vector4(p / dist * (float)MathHelper.Sin(dist), (float)MathHelper.Cos(dist));
            else
                return new Vector4(FlipXZ(p) / dist * (float)MathHelper.Sin(dist), -(float)MathHelper.Cos(dist));
        }
        if (curve < 0)
            return new Vector4(p / dist * (float)MathHelper.Sinh(dist), (float)MathHelper.Cosh(dist));
        return eucPoint;
    }

    private static Vector3 FlipXZ(Vector3 v) => new(-v.X, v.Y, -v.Z);

    /// <summary>
    /// Creates translation target valid in all geometries.
    /// Since in the hyperbolic geometry the camera is fixed, any request to change camera position must be reflected in changing the object's position.
    /// </summary>
    /// <param name="to">World-space coordinates of the translation target.</param>
    /// <param name="referencePoint">Reference point of the camera.</param>
    /// <param name="curve">Geometry curvature.</param>
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
    /// <param name="v">Vector to be reflected.</param>
    /// <param name="surfaceNormal">Normal vector of the reflection plane.</param>
    /// <returns>The reflected vector.</returns>
    public static Vector3 ReflectVector(Vector3 v, Vector3 surfaceNormal)
    {
        return v - 2 * Vector3.Dot(v, surfaceNormal) * surfaceNormal;
    }

    /// <summary>
    /// Calculates the generalized dot product
    /// </summary>
    /// <param name="u">First vector.</param>
    /// <param name="v">Second vector.</param>
    /// <param name="curve">Curvature.</param>
    /// <returns>The result of the dot product calculation.</returns>
    public static float DotProduct(Vector4 u, Vector4 v, float curve)
    {
        if (curve < 0)
            return Vector4.Dot(u, v) - 2 * u.W * v.W;

        return Vector4.Dot(u, v);
    }
}
