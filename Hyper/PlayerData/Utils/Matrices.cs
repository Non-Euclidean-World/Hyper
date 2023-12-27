﻿using Common;
using OpenTK.Mathematics;

namespace Hyper.PlayerData.Utils;

public static class Matrices
{
    /// <summary>
    /// Gets the view matrix.
    /// </summary>
    /// <param name="position">Camera position in Euclidean space</param>
    /// <param name="front">Camera's front vector in Euclidean space</param>
    /// <param name="up">Camera's up vector in Euclidean space</param>
    /// <param name="curve">If curve is equal 0 we get the matrix in Euclidean space. If it's less than 0 in hyperbolic space and if greater than 0 in spherical.</param>
    /// <returns></returns>
    public static Matrix4 ViewMatrix(Vector3 position, Vector3 front, Vector3 up, float curve, int sphere, Vector3 sphereCenter)
    {
        if (sphere == 1)
        {
            front.X *= -1;
            front.Z *= -1;
        }
        Matrix4 v = Matrix4.LookAt(position, position + front, up);
        Vector4 ic = new Vector4(v.Column0.Xyz, 0);
        Vector4 jc = new Vector4(v.Column1.Xyz, 0);
        Vector4 kc = new Vector4(v.Column2.Xyz, 0);

        Vector4 geomEye = GeomPorting.EucToCurved(position, curve, sphere, sphereCenter);

        Matrix4 eyeTranslate = TranslationMatrix(geomEye, curve, sphere);
        Vector4 icp = (sphere == 0 ? 1 : -1) * ic * eyeTranslate;
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
    /// <param name="to">Translation point</param>
    /// <param name="curve">Curvature</param>
    /// <returns>If curve is equal 0 we get the matrix in Euclidean space. If its smaller than 0 in hyperbolic space and if greater than 0 in spherical.</returns>
    public static Matrix4 TranslationMatrix(Vector4 to, float curve, int sphere)
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
            if (sphere == 0)
            {
                float denom = 1 + to.W;
                t = new Matrix4(
                1 - curve * to.X * to.X / denom, -curve * to.X * to.Y / denom, -curve * to.X * to.Z / denom, -curve * to.X,
                -curve * to.Y * to.X / denom, 1 - curve * to.Y * to.Y / denom, -curve * to.Y * to.Z / denom, -curve * to.Y,
                -curve * to.Z * to.X / denom, -curve * to.Z * to.Y / denom, 1 - curve * to.Z * to.Z / denom, -curve * to.Z,
                to.X, to.Y, to.Z, to.W);
            }
            else
            {
                float denom = 1 - to.W;
                t = new Matrix4(
                1 - curve * to.X * to.X / denom, -curve * to.X * to.Y / denom, -curve * to.X * to.Z / denom, curve * to.X,
                -curve * to.Y * to.X / denom, 1 - curve * to.Y * to.Y / denom, -curve * to.Y * to.Z / denom, curve * to.Y,
                -curve * to.Z * to.X / denom, -curve * to.Z * to.Y / denom, 1 - curve * to.Z * to.Z / denom, curve * to.Z,
                -to.X, -to.Y, -to.Z, -to.W);
            }
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
    /// <param name="curve">If curve is equal 0 we get the matrix in Euclidean space. If its smaller than 0 in hyperbolic space and if greater than 0 in spherical.</param>
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
