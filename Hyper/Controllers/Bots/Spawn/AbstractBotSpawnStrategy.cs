using Chunks;

namespace Hyper.Controllers.Bots.Spawn;

internal abstract class AbstractBotSpawnStrategy
{
    protected readonly Scene Scene;

    protected AbstractBotSpawnStrategy(Scene scene)
    {
        Scene = scene;
    }
    
    public abstract void Spawn();
    
    public abstract void Despawn();
    
    protected float GetSpawnHeight(int x, int z) // TODO remove code duplication
    {
        foreach (var chunk in Scene.Chunks)
        {
            if (x < chunk.Position.X || x > chunk.Position.X + Chunk.Size ) continue;
            if (z < chunk.Position.Z || z > chunk.Position.Z + Chunk.Size ) continue;
            var chunkX = x - chunk.Position.X;
            var chunkZ = z - chunk.Position.Z;
            bool negative = !(chunk.Voxels[chunkX, 0, chunkZ].Value >= 0);
            for (int y = 0; y < Chunk.Size; y++)
            {
                if (negative)
                {
                    if (chunk.Voxels[chunkX, y, chunkZ].Value >= 0)
                    {
                        return y + chunk.Position.Y + 1;
                    }
                }
                else
                {
                    if (chunk.Voxels[chunkX, y, chunkZ].Value < 0)
                    {
                        return y + chunk.Position.Y + 1;
                    }
                }
            }
        }

        return Chunk.Size;
    }
}