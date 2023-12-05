using Chunks;
using Chunks.ChunkManagement;
using Chunks.MarchingCubes;
using Chunks.MarchingCubes.MeshGenerators;
using FluentAssertions;
using OpenTK.Mathematics;

namespace ChunksTest.ChunkManagement;

public class SphericalChunkFactoryTests
{
    private static readonly ScalarFieldGenerator ScalarFieldGenerator = new(0);
    private Vector3i[] _sphereCenters = null!;
    private const float GlobalScale = 1;
    private SphericalMeshGenerator _meshGenerator = null!;
    private const int ChunksPerSide = 4;
    
    [SetUp]
    public void Setup()
    {
        Chunk.Size = 16;
        var sphere0Center = new Vector3i(0, 0, 0);
        var sphere1Center = new Vector3i((int)(MathF.PI / GlobalScale), 0, 0);
        _sphereCenters = new[] { sphere0Center, sphere1Center };
        _meshGenerator = new SphericalMeshGenerator(10, _sphereCenters);
    }
    
    [Test]
    public void GenerateChunk_ShouldCreateChunkWithExpectedProperties()
    {
        // Arrange
        var chunkFactory = new SphericalChunkFactory(ScalarFieldGenerator, _sphereCenters, GlobalScale, _meshGenerator);

        // Act
        var chunks = chunkFactory.CreateSpheres(ChunksPerSide, false);

        // Assert
        chunks.Should().NotBeEmpty();
    }
    
    [Test]
    public void GenerateChunk_WithSameParameters_ShouldCreateConsistentChunks()
    {
        // Arrange
        var chunkFactory1 = new SphericalChunkFactory(ScalarFieldGenerator, _sphereCenters, GlobalScale, _meshGenerator);
        var chunkFactory2 = new SphericalChunkFactory(ScalarFieldGenerator, _sphereCenters, GlobalScale, _meshGenerator);

        // Act
        var chunks1 = chunkFactory1.CreateSpheres(ChunksPerSide, false);
        var chunks2 = chunkFactory2.CreateSpheres(ChunksPerSide, false);

        // Assert
        chunks1.Should().BeEquivalentTo(chunks2);
    }
}