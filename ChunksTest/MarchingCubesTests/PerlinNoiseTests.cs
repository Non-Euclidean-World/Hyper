using Chunks.MarchingCubes;
using FluentAssertions;

namespace ChunksTest.MarchingCubesTests;

[TestFixture]
public class PerlinNoiseTests
{
    [Test]
    public void Noise3DShouldBeInValueRange()
    {
        // Arrange
        var perlin = new PerlinNoise(12345);
    
        // Act
        var noises = new List<float>();
        for (int i = 0; i < 1000; i++)
        {
            float x = i / 100.0f;
            float y = i / 200.0f;
            float z = i / 300.0f;
            noises.Add(perlin.GetNoise3D(x, y, z));
        }
    
        // Assert
        foreach (var noise in noises)
        {
            noise.Should().BeGreaterThanOrEqualTo(-1f);
            noise.Should().BeLessThanOrEqualTo(1f);
        }
    }
    
    [Test]
    public void Noise3DShouldBeDeterministic()
    {
        // Arrange
        var perlin = new PerlinNoise(12345);
    
        float x = 0.5f;
        float y = 0.6f;
        float z = 0.7f;
    
        // Act
        float noise1 = perlin.GetNoise3D(x, y, z);
        float noise2 = perlin.GetNoise3D(x, y, z);
    
        // Assert
        noise1.Should().Be(noise2);
    }
}
