using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using OpenTK.Mathematics;
using Player;
using Constants = Player.Utils.Constants;

namespace PlayerTest;

[TestFixture]
public class MatricesTests
{
    [Test]
    public void ViewMatrixShouldWorkForEuclid()
    {
        // Arrange
        var camera = new Camera(1f, 0.1f, 100f, 1f);
        camera.Curve = 0f;
        camera.ReferencePointPosition = new Vector3(0f, 0f, 0f);
        camera.Pitch = 0f;
        camera.Yaw = 0f;
        camera.Curve = 0f;

        var expected = new Matrix4(
            0f, 0f, -1f, 0f,
            0f, 1f, 0f, 0f,
            1f, 0f, 0f, 0f,
            0f, -1f, 0f, 1f);

        // Act
        var view = camera.GetViewMatrix();

        // Assert
        AreMatricesEqual(view, expected).Should().Be(true);
    }

    [Test]
    public void ProjMatrixShouldWorkForEuclid()
    {
        // Arrange
        var camera = new Camera(1f, 0.1f, 100f, 1f);
        camera.Curve = 0f;
        camera.Fov = 80;
        var aspectRatio = 1f;
        var near = 0.1f;
        var far = 100f;

        var expected = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(camera.Fov), aspectRatio, near, far);

        // Act
        var projection = camera.GetProjectionMatrix();

        // Assert
        AreMatricesEqual(projection, expected).Should().Be(true);
    }

    [Test]
    public void TranslateMatrixShouldWorkForEuclid()
    {
        // Arrange
        var camera = new Camera(1f, 0.1f, 100f, 1f);
        camera.Curve = 0f;
        camera.ReferencePointPosition = new Vector3(0f, 0f, 0f);
        camera.Pitch = 0f;
        camera.Yaw = 0f;
        camera.Curve = 0f;
        var to = new Vector4(0.5f, 1f, -0.5f, 1f);

        var expected = new Matrix4(
            1f, 0f, 0f, 0f,
            0f, 1f, 0f, 0f,
            0f, 0f, 1f, 0f,
            to.X, to.Y, to.Z, 1f);

        // Act
        var translate = camera.GetTranslationMatrix(to);

        // Assert
        AreMatricesEqual(translate, expected).Should().Be(true);
    }

    private bool AreMatricesEqual(Matrix4 a, Matrix4 b)
    {
        for (int row = 0; row < 4; row++)
        {
            for (int col = 0; col < 4; col++)
            {
                float valA = a[row, col];
                float valB = b[row, col];

                if (Math.Abs(valA - valB) > Constants.Eps)
                {
                    return false;
                }
            }
        }
        return true;
    }
}