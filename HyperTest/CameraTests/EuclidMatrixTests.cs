using FluentAssertions;
using Hyper;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace HyperTest.CameraTests
{
    [TestFixture]
    public class EuclidMatrixTests
    {
        [Test]
        public void ViewMatrixShouldWorkForEuclid()
        {
            // Arrange
            var camera = new Camera(1f, 0.1f, 100f);
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
            var camera = new Camera(1f, 0.1f, 100f);
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
            var camera = new Camera(1f, 0.1f, 100f);
            camera.Curve = 0f;
            var to = new Vector3(0.5f, 1f, -0.5f);
            var expected = Matrix4.CreateTranslation(to);

            // Act
            var to4 = camera.PortEucToCurved(to);
            var translation = camera.TranslateMatrix(to4);

            // Assert
            AreMatricesEqual(translation, expected).Should().Be(true);
        }

        private bool AreMatricesEqual(Matrix4 a, Matrix4 b, float epsilon = Constants.Eps)
        {
            for (int y = 0; y < 4; y++)
            {
                for (int x = 0; x < 4; x++)
                {
                    if (Math.Abs(a[x, y] - b[x, y]) > epsilon)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
