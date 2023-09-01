using OpenTK.Mathematics;

namespace Player.Utils;

public static class GeomPorting
{
    /// <summary>
    /// Ports a point in euclidean space to non-euclidean space.
    /// </summary>
    /// <param name="eucPoint"></param>
    /// <param name="curve">If curve is equal 0 we get the matrix in euclidean space. If its smaller than 0 in spherical space and if greater than 0 in hyperbolic.</param>
    /// <returns></returns>
    public static Vector4 EucToCurved(Vector3 eucPoint, float curve)
    {
        return EucToCurved(new Vector4(eucPoint, 1), curve);
    }

    /// <summary>
    /// Ports a point in non-euclidean space to euclidean space.
    /// </summary>
    /// <param name="eucPoint"></param>
    /// <param name="curve">If curve is equal 0 we get the matrix in euclidean space. If its smaller than 0 in spherical space and if greater than 0 in hyperbolic.</param>
    /// <returns></returns>
    public static Vector4 EucToCurved(Vector4 eucPoint, float curve)
    {
        Vector3 p = eucPoint.Xyz;
        float dist = p.Length;
        if (dist < Constants.Eps) return eucPoint;
        if (curve > 0) return new Vector4(p / dist * (float)MathHelper.Sin(dist), (float)MathHelper.Cos(dist));
        if (curve < 0) return new Vector4(p / dist * (float)MathHelper.Sinh(dist), (float)MathHelper.Cosh(dist));
        return eucPoint;
    }
}
