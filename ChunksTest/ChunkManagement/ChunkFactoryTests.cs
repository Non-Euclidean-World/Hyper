using Chunks;
using Chunks.ChunkManagement;
using Chunks.MarchingCubes;
using Chunks.MarchingCubes.MeshGenerators;
using FluentAssertions;
using OpenTK.Mathematics;

namespace ChunksTest.ChunkManagement;

public class ChunkFactoryTests
{
    private static readonly ScalarFieldGenerator ScalarFieldGenerator = new(0);
    private static readonly MeshGenerator MeshGenerator = new();

    [SetUp]
    public void Setup()
    {
        Chunk.Size = 16;
    }

    [Test]
    public void GenerateChunk_ShouldCreateChunkWithExpectedProperties()
    {
        // Arrange
        var chunkFactory = new ChunkFactory(ScalarFieldGenerator, MeshGenerator);
        var position = new Vector3i(0, 0, 0);

        // Act
        var chunk = chunkFactory.GenerateChunk(position, false);

        // Assert
        chunk.Should().NotBeNull();
        chunk.Position.Should().Be(position);
        chunk.Voxels.Should().NotBeNull();
    }

    [Test]
    public void GenerateChunk_WithSameParameters_ShouldCreateConsistentChunks()
    {
        // Arrange
        var chunkFactory1 = new ChunkFactory(ScalarFieldGenerator, MeshGenerator);
        var chunkFactory2 = new ChunkFactory(ScalarFieldGenerator, MeshGenerator);
        var position = new Vector3i(0, 0, 0);

        // Act
        var chunk1 = chunkFactory1.GenerateChunk(position, false);
        var chunk2 = chunkFactory2.GenerateChunk(position, false);

        // Assert
        chunk1.Should().BeEquivalentTo(chunk2);
    }

    [Test]
    public void GenerateChunk_WithDifferentPositions_ShouldCreateDifferentChunks()
    {
        // Arrange
        var chunkFactory = new ChunkFactory(ScalarFieldGenerator, MeshGenerator);
        var position1 = new Vector3i(0, 0, 0);
        var position2 = new Vector3i(1, 1, 1);

        // Act
        var chunk1 = chunkFactory.GenerateChunk(position1, false);
        var chunk2 = chunkFactory.GenerateChunk(position2, false);

        // Assert
        chunk1.Should().NotBeEquivalentTo(chunk2);
        chunk1.Voxels.Should().NotBeEquivalentTo(chunk2.Voxels);
    }
}