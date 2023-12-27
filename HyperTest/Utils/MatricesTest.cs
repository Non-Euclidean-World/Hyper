using Common;
using FluentAssertions;
using Hyper.PlayerData.Utils;
using OpenTK.Mathematics;

namespace HyperTest.Utils;

[TestFixture]
internal class MatricesTest
{
    [Test]
    public void ShouldGiveCorrectTranslationMatrixInSpherical()
    {
        // Arrange
        float curve = 1;
        Vector4 q = GeomPorting.EucToCurved(new Vector3(4, 5, -2), curve, 0, Vector3.Zero);
        Vector4 p = GeomPorting.EucToCurved(new Vector3(1, 2, 3), curve, 0, Vector3.Zero);

        // https://link.springer.com/article/10.1007/s00371-021-02303-2#Equ22
        Vector4 expectedTranslatedP
            = p + 2 * p.W * q - (curve * GeomPorting.DotProduct(p, q, curve) + p.W) / (q.W + 1) * (q + GeomPorting.GeometryOrigin);

        // Act
        Vector4 actualTranslatedP = p * Matrices.TranslationMatrix(q, curve, 0);

        // Assert
        AreVectorsEqual(expectedTranslatedP, actualTranslatedP).Should().BeTrue();
    }

    [Test]
    public void ShouldGiveCorrectTranslationMatrixInHyperbolic()
    {
        // Arrange
        float curve = -1;
        Vector4 q = GeomPorting.EucToCurved(new Vector3(0.4f, 1, -0.2f), curve, 0, Vector3.Zero);
        Vector4 p = GeomPorting.EucToCurved(new Vector3(1, 0.2f, 0.3f), curve, 0, Vector3.Zero);

        // https://link.springer.com/article/10.1007/s00371-021-02303-2#Equ22
        Vector4 expectedTranslatedP
            = p + 2 * p.W * q - (curve * GeomPorting.DotProduct(p, q, curve) + p.W) / (q.W + 1) * (q + GeomPorting.GeometryOrigin);

        // Act
        Vector4 actualTranslatedP = p * Matrices.TranslationMatrix(q, curve, 0);

        // Assert
        AreVectorsEqual(expectedTranslatedP, actualTranslatedP).Should().BeTrue();
    }

    [Test]
    public void ShouldGiveCorrectViewMatrixInSpherical()
    {
        // Arrange
        float curve = 1;
        Vector3 cameraPosition = new Vector3(1, 5, -3);
        Vector3 front = new Vector3(0, 0, 1);
        Vector3 up = new Vector3(0, 1, 0);

        Vector4 ic = new Vector4(Vector3.Cross(front, up), 0);
        Vector4 jc = new Vector4(up, 0);
        Vector4 kc = new Vector4(-front, 0);
        Vector4 geomEye = GeomPorting.EucToCurved(cameraPosition, curve, 0, Vector3.Zero);

        Matrix4 eyeTranslate = Matrices.TranslationMatrix(geomEye, curve, 0);
        Vector4 icp = ic * eyeTranslate;
        Vector4 jcp = jc * eyeTranslate;
        Vector4 kcp = kc * eyeTranslate;

        // Act
        Matrix4 view = Matrices.ViewMatrix(cameraPosition, front, up, curve, 0, Vector3.Zero);

        // Assert
        // https://link.springer.com/article/10.1007/s00371-021-02303-2#Equ27
        AreScalarsEqual(GeomPorting.DotProduct(geomEye, icp, curve), 0).Should().BeTrue();
        AreScalarsEqual(GeomPorting.DotProduct(geomEye, jcp, curve), 0).Should().BeTrue();
        AreScalarsEqual(GeomPorting.DotProduct(geomEye, kcp, curve), 0).Should().BeTrue();
        AreVectorsEqual(geomEye * view, GeomPorting.GeometryOrigin).Should().BeTrue();
    }

    [Test]
    public void ShouldGiveCorrectViewMatrixInHyperbolic()
    {
        // Arrange
        float curve = -1;
        Vector3 cameraPosition = new Vector3(1, 5, -3);
        Vector3 front = new Vector3(0, 0, 1);
        Vector3 up = new Vector3(0, 1, 0);

        Vector4 ic = new Vector4(Vector3.Cross(front, up), 0);
        Vector4 jc = new Vector4(up, 0);
        Vector4 kc = new Vector4(-front, 0);
        Vector4 geomEye = GeomPorting.EucToCurved(cameraPosition, curve, 0, Vector3.Zero);

        Matrix4 eyeTranslate = Matrices.TranslationMatrix(geomEye, curve, 0);
        Vector4 icp = ic * eyeTranslate;
        Vector4 jcp = jc * eyeTranslate;
        Vector4 kcp = kc * eyeTranslate;

        // Act
        Matrix4 view = Matrices.ViewMatrix(cameraPosition, front, up, curve, 0, Vector3.Zero);

        // Assert
        // https://link.springer.com/article/10.1007/s00371-021-02303-2#Equ27
        AreScalarsEqual(GeomPorting.DotProduct(geomEye, icp, curve), 0).Should().BeTrue();
        AreScalarsEqual(GeomPorting.DotProduct(geomEye, jcp, curve), 0).Should().BeTrue();
        AreScalarsEqual(GeomPorting.DotProduct(geomEye, kcp, curve), 0).Should().BeTrue();
        AreVectorsEqual(geomEye * view, GeomPorting.GeometryOrigin).Should().BeTrue();
    }

    private static bool AreScalarsEqual(float a, float b, float eps = 0.001f)
    {
        return MathF.Abs(a - b) < eps;
    }

    private static bool AreVectorsEqual(Vector4 a, Vector4 b, float eps = 0.001f)
    {
        return MathF.Abs(a.X - b.X) < eps
            && MathF.Abs(a.Y - b.Y) < eps
            && MathF.Abs(a.Z - b.Z) < eps
            && MathF.Abs(a.W - b.W) < eps;
    }
}
