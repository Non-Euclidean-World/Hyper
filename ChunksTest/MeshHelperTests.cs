using BepuUtilities.Memory;
using Chunks;
using Common.Meshes;
using FluentAssertions;
using OpenTK.Mathematics;

namespace ChunksTest;

public class MeshHelperTests
{
    [Test]
    public void CreateCollisionSurface_WithValidMesh_ShouldReturnValidBepuMesh()
    {
        // Arrange
        var vertices = new Vertex[]
        {
            new(0, 0, 0, 0, 0, 0),
            new(1, 0, 0, 0, 0, 0),
            new(0, 1, 0, 0, 0, 0),
            // Add more vertices as needed
        };
        var mesh = new Mesh(vertices, new Vector3(0, 0, 0), false);
        var pool = new BufferPool();

        // Act
        var result = MeshHelper.CreateCollisionSurface(mesh, pool);

        // Assert
        result.Should().NotBeNull();
        result!.Value.Triangles.Length.Should().Be(vertices.Length / 3);
    }

    [Test]
    public void CreateCollisionSurface_WithEmptyMesh_ShouldReturnNull()
    {
        // Arrange
        var emptyVertices = Array.Empty<Vertex>();
        var emptyMesh = new Mesh(emptyVertices, new Vector3(0, 0, 0), false);
        var pool = new BufferPool();

        // Act
        var result = MeshHelper.CreateCollisionSurface(emptyMesh, pool);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public void CreateCollisionSurface_WithNullMesh_ShouldThrowArgumentNullException()
    {
        // Arrange
        Mesh nullMesh = null!;
        var pool = new BufferPool();

        // Act
        Action act = () => MeshHelper.CreateCollisionSurface(nullMesh, pool);

        // Assert
        act.Should().Throw<NullReferenceException>();
    }
}