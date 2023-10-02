using Common;
using FluentAssertions;
using OpenTK.Mathematics;
using Player.Utils;

namespace PlayerTest;

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
        AreVectorsEqual(vector, curved).Should().Be(true);
    }

    private static bool AreVectorsEqual(Vector4 a, Vector4 b)
    {
        if (MathF.Abs(a.X - b.X) > Constants.Eps) return false;
        if (MathF.Abs(a.Y - b.Y) > Constants.Eps) return false;
        if (MathF.Abs(a.Z - b.Z) > Constants.Eps) return false;
        if (MathF.Abs(a.W - b.W) > Constants.Eps) return false;

        return true;
    }
}