using OpenTK.Mathematics;
using Hyper;
using FluentAssertions;

namespace HyperTest.CameraTests
{
    [TestFixture]
    public class UpdatePositionTests
    {
        [Test]
        public void UpdatePositionShouldMoveCamera()
        {
            // Arrange
            var camera = new Camera(new Vector3(0, 0, 0), 1);

            // Act
            camera.UpdatePosition(new Vector3(1, 0, 0));

            // Assert
            camera.Position.Should().Be(new Vector3(1, 0, 0));
        }
    }
}
