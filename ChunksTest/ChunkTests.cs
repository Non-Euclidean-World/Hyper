using Chunks;
using Chunks.Voxels;
using Common.Meshes;
using FluentAssertions;
using OpenTK.Mathematics;

namespace ChunksTest;

public class ChunkTests
{
    private const int ChunkSize = 5;
    private static readonly Vector3i ChunkPosition = Vector3i.Zero;
    
    [SetUp]
    public void SetUp()
    {
        Chunk.Size = ChunkSize;
    }

    [Test]
    public void MineShouldUpdateMeshInsideRadius()
    {
        // Arrange
        var chunk = GetChunk();
        var location = new Vector3(0.5f, 0.5f, 0.5f);
        var deltaTime = 0.1f;
        var brushWeight = 0.5f;
        var radius = 1;

        // Act
        chunk.Mine(location, deltaTime, brushWeight, radius);

        // Assert
        chunk.Voxels[0, 0, 0].Value.Should().BeGreaterThan(0);
        chunk.Voxels[1, 0, 0].Value.Should().BeGreaterThan(0);
        chunk.Voxels[0, 1, 0].Value.Should().BeGreaterThan(0);
        chunk.Voxels[0, 0, 1].Value.Should().BeGreaterThan(0);
    }
    
    [Test]
    public void MineShouldNotUpdateMeshOutsideRadius()
    {
        // Arrange
        var chunk = GetChunk();
        var location = new Vector3(0.5f, 0.5f, 0.5f);
        var deltaTime = 0.1f;
        var brushWeight = 0.5f;
        var radius = 1;

        // Act
        chunk.Mine(location, deltaTime, brushWeight, radius);

        // Assert
        chunk.Voxels[3, 3, 3].Value.Should().Be(0);
    }
    
    [Test]
    public void BuildShouldUpdateMeshInsideRadius()
    {
        // Arrange
        var chunk = GetChunk();
        var location = new Vector3(0.5f, 0.5f, 0.5f);
        var deltaTime = 0.1f;
        var brushWeight = 0.5f;
        var radius = 1;

        // Act
        chunk.Build(location, deltaTime, brushWeight, radius);

        // Assert
        chunk.Voxels[0, 0, 0].Value.Should().BeLessThan(0);
        chunk.Voxels[1, 0, 0].Value.Should().BeLessThan(0);
        chunk.Voxels[0, 1, 0].Value.Should().BeLessThan(0);
        chunk.Voxels[0, 0, 1].Value.Should().BeLessThan(0);
    }
    
    [Test]
    public void BuildShouldNotUpdateMeshOutsideRadius()
    {
        // Arrange
        var chunk = GetChunk();
        var location = new Vector3(0.5f, 0.5f, 0.5f);
        var deltaTime = 0.1f;
        var brushWeight = 0.5f;
        var radius = 1;

        // Act
        chunk.Build(location, deltaTime, brushWeight, radius);

        // Assert
        chunk.Voxels[3, 3, 3].Value.Should().Be(0);
    }
    
    [Test]
    public void DistanceFromChunkShouldReturnZeroWhenInside()
    {
        // Arrange
        var chunk = GetChunk();
        var location = new Vector3(0.5f, 0.5f, 0.5f);

        // Act
        var distance = chunk.DistanceFromChunk(location);

        // Assert
        distance.Should().Be(0);
    }
    
    [Test]
    public void DistanceFromChunkShouldReturnDistance()
    {
        // Arrange
        var chunk = GetChunk();
        var locationDistanceMap = new List<(Vector3 Location, float ExpectedDistance)>()
        {
            new (new Vector3(0, 0, 0), 0f),
            new (new Vector3(6, 6, 6), 0f),
            new (new Vector3(3, 3, 3), 0f),
            new (new Vector3(10, 0, 0), 2f),
            new (new Vector3(-1, -1, -1), 1f),
        };

        // Act
        var distances = locationDistanceMap.Select(location => chunk.DistanceFromChunk(location.Location)).ToList();

        // Assert
        for (int i = 0; i < locationDistanceMap.Count; i++)
        {
            distances[i].Should().Be(locationDistanceMap[i].ExpectedDistance);
        }
    }

    private Chunk GetChunk()
    {
        var voxels = new Voxel[ChunkSize, ChunkSize, ChunkSize];
        return new Chunk(Array.Empty<Vertex>(), ChunkPosition, voxels, 0, false);
    }
}