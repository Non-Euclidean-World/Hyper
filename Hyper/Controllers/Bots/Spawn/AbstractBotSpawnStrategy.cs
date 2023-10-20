using Chunks;
using Chunks.MarchingCubes.MeshGenerators;
using Common;

namespace Hyper.Controllers.Bots.Spawn;

internal abstract class AbstractBotSpawnStrategy
{
    protected readonly Scene Scene;

    protected readonly Random Rand;

    protected AbstractBotSpawnStrategy(Scene scene, Settings settings)
    {
        Scene = scene;
        Rand = new Random(settings.Seed);
    }

    public abstract void Spawn();

    public abstract void Despawn();

    protected float GetSpawnHeight(int x, int z)
    {
        foreach (var chunk in Scene.Chunks)
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
}