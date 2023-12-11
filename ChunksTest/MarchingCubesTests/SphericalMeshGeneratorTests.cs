using Chunks.ChunkManagement;
using Chunks.MarchingCubes.MeshGenerators;
using Chunks.Voxels;
using FluentAssertions;
using OpenTK.Mathematics;

namespace ChunksTest.MarchingCubesTests;

public class SphericalMeshGeneratorTests
{
    [Test]
    public void GetMesh_WithEmptyScalarField_ShouldReturnEmptyVertexArray()
    {
        // Arrange
        var sphere0Center = new Vector3i(0, 0, 0);
        var sphere1Center = new Vector3i((int)(MathF.PI), 0, 0);
        var sphereCenters = new[] { sphere0Center, sphere1Center };
        var meshGenerator = new SphericalMeshGenerator(0, sphereCenters);
        var emptyScalarField = new Voxel[1, 1, 1];
        var chunkData = new ChunkData { Voxels = emptyScalarField };
        var chunkPosition = new Vector3i(0, 0, 0);

        // Act
        var result = meshGenerator.GetMesh(chunkPosition, chunkData);

        // Assert
        result.Should().BeEmpty();
    }

    [Test]
    public void GetMesh_WithNonEmptyScalarField_ShouldReturnNonEmptyVertexArray()
    {
        // Arrange
        var sphere0Center = new Vector3i(0, 0, 0);
        var sphere1Center = new Vector3i((int)(MathF.PI), 0, 0);
        var sphereCenters = new[] { sphere0Center, sphere1Center };
        var meshGenerator = new SphericalMeshGenerator(10, sphereCenters);
        var scalarField = new Voxel[8, 8, 8];
        scalarField[1, 3, 1] = new Voxel(-1, VoxelType.Grass);
        var chunkData = new ChunkData { Voxels = scalarField };
        var chunkPosition = new Vector3i(0, 0, 0);

        // Act
        var result = meshGenerator.GetMesh(chunkPosition, chunkData);

        // Assert
        result.Should().NotBeEmpty();
    }

    [Test]
    public void GetMesh_ShouldReturnEmpty_WhenOutsideRadius()
    {
        // Arrange
        var sphere0Center = new Vector3i(0, 0, 0);
        var sphere1Center = new Vector3i((int)(MathF.PI), 0, 0);
        var sphereCenters = new[] { sphere0Center, sphere1Center };
        var meshGenerator = new SphericalMeshGenerator(0, sphereCenters);
        var scalarField = new Voxel[8, 8, 8];
        scalarField[1, 3, 1] = new Voxel(-1, VoxelType.Grass);
        var chunkData = new ChunkData { Voxels = scalarField };
        var chunkPosition = new Vector3i(0, 0, 0);

        // Act
        var result = meshGenerator.GetMesh(chunkPosition, chunkData);

        // Assert
        result.Should().BeEmpty();
    }
}