using Chunks;
using Chunks.MarchingCubes.MeshGenerators;
using OpenTK.Mathematics;

namespace Hyper.Controllers.Bots.Spawn;

/// <summary>
/// Utility methods for spawning game objects.
/// </summary>
internal static class SpawnUtils
{
    /// <summary>
    /// Gets the spawn height for a game objects with given planar coordinates.
    /// </summary>
    /// <param name="x">The x spawn coordinate.</param>
    /// <param name="z">The z spawn coordinate.</param>
    /// <param name="scene">The scene where the object is to be spawned.</param>
    /// <returns>The height of the terrain.</returns>
    public static float GetSpawnHeight(int x, int z, Scene scene)
    {
        foreach (var chunk in scene.Chunks)
        {
            if (x < chunk.Position.X || x > chunk.Position.X + Chunk.Size) continue;
            if (z < chunk.Position.Z || z > chunk.Position.Z + Chunk.Size) continue;
            var chunkX = x - chunk.Position.X;
            var chunkZ = z - chunk.Position.Z;
            bool negative = chunk.Voxels[chunkX, 0, chunkZ].Value < BaseMeshGenerator.IsoLevel;
            for (int y = 0; y < Chunk.Size; y++)
            {
                if (negative)
                {
                    if (chunk.Voxels[chunkX, y, chunkZ].Value >= BaseMeshGenerator.IsoLevel)
                    {
                        return y + chunk.Position.Y + 1;
                    }
                }
                else
                {
                    if (chunk.Voxels[chunkX, y, chunkZ].Value < BaseMeshGenerator.IsoLevel)
                    {
                        return y + chunk.Position.Y + 1;
                    }
                }
            }
        }

        return Chunk.Size;
    }


    /// <summary>
    /// Gets the spawn location for a game objects with given planar coordinates.
    /// </summary>
    /// <param name="x">The x spawn coordinate.</param>
    /// <param name="z">The z spawn coordinate.</param>
    /// <param name="scene">The scene where the object is to be spawned.</param>
    /// <returns>The possible spawn location with given (x, z) coordinates.</returns>
    public static Vector3 GetSpawnLocation(int x, int z, Scene scene)
    {
        return new Vector3(x, GetSpawnHeight(x, z, scene), z);
    }
}
