using Chunks;
using Chunks.MarchingCubes;
using FluentAssertions;
using OpenTK.Mathematics;

namespace ChunksTest.MarchingCubesTests;

public class ScalarFieldGeneratorTests
{
    public static readonly int Extra = Chunk.Overlap + 1;
    
    [Test]
    public void Generate_WithSpecificSize_ShouldCreateCorrectlySizedVoxelArray()
    {
        // Arrange
        var generator = new ScalarFieldGenerator(123);
        int width = 10, height = 10, depth = 10;
        var position = new Vector3i(0, 0, 0);

        // Act
        var voxelArray = generator.Generate(width, height, depth, position);

        // Assert
        voxelArray.GetLength(0).Should().Be(width + Extra);
        voxelArray.GetLength(1).Should().Be(height + Extra);
        voxelArray.GetLength(2).Should().Be(depth + Extra);
    }
    
    [Test]
    public void Generate_WithUniformSize_ShouldCreateCorrectlySizedVoxelArray()
    {
        // Arrange
        var generator = new ScalarFieldGenerator(123);
        int size = 15;
        var position = new Vector3i(0, 0, 0);

        // Act
        var voxelArray = generator.Generate(size, position);

        // Assert
        voxelArray.GetLength(0).Should().Be(size + Extra);
        voxelArray.GetLength(1).Should().Be(size + Extra);
        voxelArray.GetLength(2).Should().Be(size + Extra);
    }

    [Test]
    public void Generate_WithSameSeed_ShouldProduceIdenticalScalarFields()
    {
        // Arrange
        int seed = 123;
        var generator1 = new ScalarFieldGenerator(seed);
        var generator2 = new ScalarFieldGenerator(seed);
        int size = 10;
        var position = new Vector3i(0, 0, 0);

        // Act
        var scalarField1 = generator1.Generate(size, position);
        var scalarField2 = generator2.Generate(size, position);

        // Assert
        scalarField1.Should().BeEquivalentTo(scalarField2);
    }

    [Test]
    public void Generate_WithDifferentSeeds_ShouldProduceDifferentScalarFields()
    {
        // Arrange
        int seed1 = 123;
        int seed2 = 456;
        var generator1 = new ScalarFieldGenerator(seed1);
        var generator2 = new ScalarFieldGenerator(seed2);
        int size = 10;
        var position = new Vector3i(0, 0, 0);

        // Act
        var scalarField1 = generator1.Generate(size, position);
        var scalarField2 = generator2.Generate(size, position);

        // Assert
        scalarField1.Should().NotBeEquivalentTo(scalarField2);
    }

}