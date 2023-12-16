using System.Numerics;
using BepuUtilities;

namespace Hyper;
internal static class Helpers
{
    /// <summary>
    /// Creates a quaternion representing a rotation which rotates one vector onto another
    /// following the shortest arc.
    /// </summary>
    /// <param name="source">Rotated vector</param>
    /// <param name="target">Vector which the <c>source</c> should be aligned with</param>
    /// <returns>Quaternion representing</returns>
    public static Quaternion CreateQuaternionFromTwoVectors(Vector3 source, Vector3 target)
    {
#if DEBUG
        const float eps = 0.0001f;
        if (source.LengthSquared() < eps || target.LengthSquared() < eps)
            throw new ArgumentException("Neither source nor target vector can be zero");
#endif

        float angle = MathF.Acos(Vector3.Dot(source, target));
        Vector3 axis = Vector3.Normalize(Vector3.Cross(source, target));

        return QuaternionEx.CreateFromAxisAngle(axis, angle);
    }
}
