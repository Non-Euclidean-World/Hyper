using Common;
using FluentAssertions;
using OpenTK.Mathematics;

namespace CommonTest;
[TestFixture]
public class GeomPortingTests
{
    [Test]
    public void ShouldNotChangeForCurveZero()
    {
        // Arrange
        var vector = new Vector4(1, 2, 4, 1);

        // Act
        var curved = GeomPorting.EucToCurved(vector, 0);

        // Assert
        vector.Should().BeEquivalentTo(curved);
    }

    [Test]
    public void ShouldPortOntoASphere()
    {
        // Arrange
        var vector = new Vector3(68, 420, 100);

        // Act
        var curved = GeomPorting.EucToCurved(vector, 1);

        // Assert
        IsOnUnitSphere(curved).Should().BeTrue();
    }

    [Test]
    public void ShouldPortOntoAHyperboloid()
    {
        // Arrange
        var vector = new Vector3(3.14f, 3.20f, 1.0f);

        // Act
        var curved = GeomPorting.EucToCurved(vector, -1);

        // Assert
        IsOnUnitHyperboloid(curved).Should().BeTrue();
    }

    [Test]
    public void ShouldReflectVector()
    {
        // Arrange
        var v = new Vector3(1, 1, 1);
        var surfaceNormal = new Vector3(0, 0, 1);

        // Act
        var reflected = GeomPorting.ReflectVector(v, surfaceNormal);

        // Assert
        reflected.Should().BeEquivalentTo(new Vector3(1, 1, -1));
    }

    [Test]
    public void ShouldLeaveVectorOnPlaneUnchanged()
    {
        // Arrange
        var v = new Vector3(1, 1, 0);
        var surfaceNormal = new Vector3(0, 0, 1);

        // Act
        var reflected = GeomPorting.ReflectVector(v, surfaceNormal);

        // Assert
        reflected.Should().BeEquivalentTo(new Vector3(1, 1, 0));
    }

    [Test]
    public void DifferentNormalsOfTheSamePlaneShouldYieldTheSameReflection()
    {
        // Arrange
        var v = new Vector3(14, 15, -23);
        var surfaceNormal1 = new Vector3(0, 0, 1);
        var surfaceNormal2 = new Vector3(0, 0, -1);

        // Act
        var reflected1 = GeomPorting.ReflectVector(v, surfaceNormal1);
        var reflected2 = GeomPorting.ReflectVector(v, surfaceNormal2);

        // Assert
        reflected1.Should().BeEquivalentTo(reflected2);
    }

    private static bool IsOnUnitSphere(Vector4 v)
    {
        return MathF.Abs(v.LengthSquared - 1) < 0.001;
    }

    private static bool IsOnUnitHyperboloid(Vector4 v)
    {
        return MathF.Abs(v.X * v.X + v.Y * v.Y + v.Z * v.Z - v.W * v.W + 1) < 0.001;
    }
}