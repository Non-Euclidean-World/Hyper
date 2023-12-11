using Character;
using FluentAssertions;

namespace CharactersTest;

public class AssimpConversionsTest
{
    [Test]
    public void ToNumericsVectorTest()
    {
        // Arrange
        var v = new Assimp.Vector3D(1, 2, 3);
        var expected = new System.Numerics.Vector3(1, 2, 3);

        // Act
        var actual = AssimpConversions.ToNumericsVector(v);

        // Assert
        actual.X.Should().Be(expected.X);
        actual.Y.Should().Be(expected.Y);
        actual.Z.Should().Be(expected.Z);
    }

    [Test]
    public void ToOpenTkVectorTest()
    {
        // Arrange
        var v = new Assimp.Vector3D(1, 2, 3);
        var expected = new OpenTK.Mathematics.Vector3(1, 2, 3);

        // Act
        var actual = AssimpConversions.ToOpenTKVector(v);

        // Assert
        actual.X.Should().Be(expected.X);
        actual.Y.Should().Be(expected.Y);
        actual.Z.Should().Be(expected.Z);
    }

    [Test]
    public void ToOpenTkMatrixTest()
    {
        // Arrange
        var m = new Assimp.Matrix4x4(
            1, 2, 3, 4,
            5, 6, 7, 8,
            9, 10, 11, 12,
            13, 14, 15, 16);
        var expected = new OpenTK.Mathematics.Matrix4(
            1, 2, 3, 4,
            5, 6, 7, 8,
            9, 10, 11, 12,
            13, 14, 15, 16);

        // Act
        var actual = AssimpConversions.ToOpenTKMatrix(m);

        // Assert
        actual.M11.Should().Be(expected.M11);
        actual.M12.Should().Be(expected.M12);
        actual.M13.Should().Be(expected.M13);
        actual.M14.Should().Be(expected.M14);
        actual.M21.Should().Be(expected.M21);
        actual.M22.Should().Be(expected.M22);
        actual.M23.Should().Be(expected.M23);
        actual.M24.Should().Be(expected.M24);
        actual.M31.Should().Be(expected.M31);
        actual.M32.Should().Be(expected.M32);
        actual.M33.Should().Be(expected.M33);
        actual.M34.Should().Be(expected.M34);
        actual.M41.Should().Be(expected.M41);
        actual.M42.Should().Be(expected.M42);
        actual.M43.Should().Be(expected.M43);
        actual.M44.Should().Be(expected.M44);
    }
}