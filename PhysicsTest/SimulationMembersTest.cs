using BepuPhysics;
using FluentAssertions;
using Moq;
using Physics;
using Physics.Collisions;

namespace PhysicsTest;

[TestFixture]
internal class SimulationMembersTest
{
    [Test]
    public void ShouldAddBodyHandles()
    {
        // Arrange
        SimulationMembers subject = new SimulationMembers();
        Mock<ISimulationMember> mockSimulationMember = new Mock<ISimulationMember>();
        var bodyHandles = new List<BodyHandle> { new(1), new(2) };
        mockSimulationMember.Setup(mock => mock.BodyHandles).Returns(bodyHandles);

        // Act
        subject.Add(mockSimulationMember.Object);

        // Assert
        subject.Contains(new BodyHandle(1)).Should().BeTrue();
        subject.Contains(new BodyHandle(2)).Should().BeTrue();
        subject.Contains(new BodyHandle(3)).Should().BeFalse();
    }

    [Test]
    public void ShouldRemoveBodyHandles()
    {
        // Arrange
        SimulationMembers subject = new SimulationMembers();
        Mock<ISimulationMember> mockSimulationMember1 = new Mock<ISimulationMember>();
        Mock<ISimulationMember> mockSimulationMember2 = new Mock<ISimulationMember>();
        var bodyHandles1 = new List<BodyHandle> { new(1), new(2) };
        var bodyHandles2 = new List<BodyHandle> { new(3), new(4) };
        mockSimulationMember1.Setup(mock => mock.BodyHandles).Returns(bodyHandles1);
        mockSimulationMember2.Setup(mock => mock.BodyHandles).Returns(bodyHandles2);

        // Act
        subject.Add(mockSimulationMember1.Object);
        subject.Add(mockSimulationMember2.Object);

        subject.Remove(mockSimulationMember1.Object);

        // Assert
        subject.Contains(new(1)).Should().BeFalse();
        subject.Contains(new(2)).Should().BeFalse();
        subject.Contains(new(3)).Should().BeTrue();
        subject.Contains(new(4)).Should().BeTrue();
    }

    [Test]
    public void ShouldRetrieveByHandle()
    {
        // Arrange
        SimulationMembers subject = new SimulationMembers();
        Mock<ISimulationMember> mockSimulationMember = new Mock<ISimulationMember>();
        var bodyHandles = new List<BodyHandle> { new(1), new(2) };
        mockSimulationMember.Setup(mock => mock.BodyHandles).Returns(bodyHandles);

        // Act
        subject.Add(mockSimulationMember.Object);
        bool result1 = subject.TryGetByHandle(new(1), out var member1);
        bool result2 = subject.TryGetByHandle(new(3), out var member2);

        // Assert
        result1.Should().BeTrue();
        member1.Should().Be(mockSimulationMember.Object);

        result2.Should().BeFalse();
        member2.Should().BeNull();
    }

    [Test]
    public void ShouldAccessByBodyHandle()
    {
        // Arrange
        SimulationMembers subject = new SimulationMembers();
        Mock<ISimulationMember> mockSimulationMember = new Mock<ISimulationMember>();
        var bodyHandles = new List<BodyHandle> { new(1), new(2) };
        mockSimulationMember.Setup(mock => mock.BodyHandles).Returns(bodyHandles);

        // Act
        subject.Add(mockSimulationMember.Object);
        ISimulationMember result = subject[new(1)];

        // Assert
        result.Should().Be(mockSimulationMember.Object);
    }
}
