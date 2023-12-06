using FluentAssertions;
using Physics.TypingUtils;

namespace PhysicsTest.TypingUtils;

[TestFixture]
internal class ConversionsTest
{
    [Test]
    public void ShouldConvertOpenTKVec2ToNumericsVec2()
    {
        // Arrange
        OpenTK.Mathematics.Vector2 vector2 = new OpenTK.Mathematics.Vector2(3.14f, -2.72f);

        // Act
        System.Numerics.Vector2 result = Conversions.ToNumericsVector(vector2);

        // Assert
        result.X.Should().Be(vector2.X);
        result.Y.Should().Be(vector2.Y);
    }

    [Test]
    public void ShouldConvertOpenTKVec3ToNumericsVec3()
    {
        // Arrange
        OpenTK.Mathematics.Vector3 vector3 = new OpenTK.Mathematics.Vector3(3.14f, -2.72f, 1.62f);

        // Act
        System.Numerics.Vector3 result = Conversions.ToNumericsVector(vector3);

        // Assert
        result.X.Should().Be(vector3.X);
        result.Y.Should().Be(vector3.Y);
        result.Z.Should().Be(vector3.Z);
    }

    [Test]
    public void ShouldConvertNumericsVec2ToOpenTKVec2()
    {
        // Arrange
        System.Numerics.Vector2 vector2 = new System.Numerics.Vector2(3.14f, -2.72f);

        // Act
        OpenTK.Mathematics.Vector2 result = Conversions.ToOpenTKVector(vector2);

        // Assert
        result.X.Should().Be(vector2.X);
        result.Y.Should().Be(vector2.Y);
    }

    [Test]
    public void ShouldConvertNumericsVec3ToOpenTKVec3()
    {
        // Arrange
        System.Numerics.Vector3 vector3 = new System.Numerics.Vector3(3.14f, -2.72f, 1.62f);

        // Act
        OpenTK.Mathematics.Vector3 result = Conversions.ToOpenTKVector(vector3);

        // Assert
        result.X.Should().Be(vector3.X);
        result.Y.Should().Be(vector3.Y);
        result.Z.Should().Be(vector3.Z);
    }

    [Test]
    public void ShouldConvertNumericsMat4ToOpenTKMat4()
    {
        // Arrange
        System.Numerics.Matrix4x4 matrix4 = new System.Numerics.Matrix4x4(
            0.1f, 0.2f, 0.3f, 0.4f,
            0.5f, 0.6f, 0.7f, 0.8f,
            -0.1f, -0.2f, -0.3f, -0.4f,
           -0.5f, -0.6f, -0.7f, -0.8f);

        // Act
        OpenTK.Mathematics.Matrix4 result = Conversions.ToOpenTKMatrix(matrix4);

        // Assert
        result.M11.Should().Be(matrix4.M11);
        result.M12.Should().Be(matrix4.M12);
        result.M13.Should().Be(matrix4.M13);
        result.M14.Should().Be(matrix4.M14);

        result.M21.Should().Be(matrix4.M21);
        result.M22.Should().Be(matrix4.M22);
        result.M23.Should().Be(matrix4.M23);
        result.M24.Should().Be(matrix4.M24);

        result.M31.Should().Be(matrix4.M31);
        result.M32.Should().Be(matrix4.M32);
        result.M33.Should().Be(matrix4.M33);
        result.M34.Should().Be(matrix4.M34);

        result.M41.Should().Be(matrix4.M41);
        result.M42.Should().Be(matrix4.M42);
        result.M43.Should().Be(matrix4.M43);
        result.M44.Should().Be(matrix4.M44);
    }
}
