using OpenTK.Mathematics;

namespace Hyper.MathUtiils;

internal static class GeomPorting
{
    public static Vector4 EucToCurved(Vector3 eucPoint, float curve)
    {
        return EucToCurved(new Vector4(eucPoint, 1), curve);
    }

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
