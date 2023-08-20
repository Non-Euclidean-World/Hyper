using OpenTK.Mathematics;

namespace Hyper.MathUtils;

internal static class Matrices
{
    /// <summary>
    /// Gets the view matrix.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="front"></param>
    /// <param name="up"></param>
    /// <param name="curve">If curve is equal 0 we get the matrix in euclidean space. If its smaller than 0 in spherical space and if greater than 0 in hyperbolic.</param>
    /// <returns></returns>
    public static Matrix4 ViewMatrix(Vector3 position, Vector3 front, Vector3 up, float curve)
    {
        Matrix4 v = Matrix4.LookAt(position, position + front, up);
        Vector4 ic = new Vector4(v.Column0.Xyz, 0);
        Vector4 jc = new Vector4(v.Column1.Xyz, 0);
        Vector4 kc = new Vector4(v.Column2.Xyz, 0);

        Vector4 geomEye = GeomPorting.EucToCurved(position, curve);

        Matrix4 eyeTranslate = TranslationMatrix(geomEye, curve);
        Vector4 icp = ic * eyeTranslate;
        Vector4 jcp = jc * eyeTranslate;
        Vector4 kcp = kc * eyeTranslate;

        if (MathHelper.Abs(curve) < Constants.Eps)
        {
            return v;
        }

        Matrix4 nonEuclidView = new Matrix4(
            icp.X, jcp.X, kcp.X, curve * geomEye.X,
            icp.Y, jcp.Y, kcp.Y, curve * geomEye.Y,
        icp.Z, jcp.Z, kcp.Z, curve * geomEye.Z,
        curve * icp.W, curve * jcp.W, curve * kcp.W, geomEye.W);

        return nonEuclidView;
    }

    /// <summary>
    /// Gets the translation matrix.
    /// </summary>
    /// <param name="to"></param>
    /// <param name="curve"></param>
    /// <returns>If curve is equal 0 we get the matrix in euclidean space. If its smaller than 0 in spherical space and if greater than 0 in hyperbolic.</returns>
    public static Matrix4 TranslationMatrix(Vector4 to, float curve)
    {
        Matrix4 t;
        if (MathHelper.Abs(curve) < Constants.Eps)
        {
            t = new Matrix4(
            1, 0, 0, 0,
            0, 1, 0, 0,
            0, 0, 1, 0,
            to.X, to.Y, to.Z, 1);
        }
        else
        {
            float denom = 1 + to.W;
            t = new Matrix4(
            1 - curve * to.X * to.X / denom, -curve * to.X * to.Y / denom, -curve * to.X * to.Z / denom, -curve * to.X,
            -curve * to.Y * to.X / denom, 1 - curve * to.Y * to.Y / denom, -curve * to.Y * to.Z / denom, -curve * to.Y,
            -curve * to.Z * to.X / denom, -curve * to.Z * to.Y / denom, 1 - curve * to.Z * to.Z / denom, -curve * to.Z,
            to.X, to.Y, to.Z, to.W);
        }
        return t;
    }

    /// <summary>
    /// Gets the projection matrix.
    /// </summary>
    /// <param name="fov"></param>
    /// <param name="near"></param>
    /// <param name="far"></param>
    /// <param name="aspectRatio"></param>
    /// <param name="curve">If curve is equal 0 we get the matrix in euclidean space. If its smaller than 0 in spherical space and if greater than 0 in hyperbolic.</param>
    /// <returns></returns>
    public static Matrix4 ProjectionMatrix(float fov, float near, float far, float aspectRatio, float curve)
    {
        Matrix4 p = Matrix4.CreatePerspectiveFieldOfView(fov, aspectRatio, near, far);
        float sFovX = p.Column0.X;
        float sFovY = p.Column1.Y;
        float fp = near; // scale front clipping plane according to the global scale factor of the scene

        if (curve <= Constants.Eps)
        {
            return p;
        }
        Matrix4 nonEuclidProj = new Matrix4(
            sFovX, 0, 0, 0,
            0, sFovY, 0, 0,
            0, 0, 0, -1,
            0, 0, -fp, 0
            );

        return nonEuclidProj;
    }
}
