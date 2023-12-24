using Chunks;
using Common;
using Hyper.GameEntities;
using OpenTK.Mathematics;

namespace Hyper.Controllers.Bots.Spawn;

internal class SphericalBotSpawnStrategy : AbstractBotSpawnStrategy
{
    private const int MaxBots = 0;

    private static int DistanceFromChunkCenter => Chunk.Size / 4;

    public SphericalBotSpawnStrategy(Scene scene, Settings settings) : base(scene, settings) { }

    public override void Spawn()
    {
        while (Scene.Bots.Count < MaxBots)
        {
            var chunk = GetRandomChunk(Rand);
            var x = Chunk.Size / 2 + Rand.Next(-DistanceFromChunkCenter, DistanceFromChunkCenter);
            var z = Chunk.Size / 2 + Rand.Next(-DistanceFromChunkCenter, DistanceFromChunkCenter);
            var position = chunk.Position + new Vector3(x, 0, z);
            position.Y = SpawnUtils.GetSpawnHeight((int)position.X, (int)position.Z, Scene);
            var bot = new AstronautBot(Humanoid.CreatePhysicalCharacter(position, Scene.SimulationManager), Scene.SphereCenters)
            {
                CurrentSphereId = chunk.Sphere
            };
#if DEBUG
            Console.WriteLine($"Spawning bot {bot.BodyHandle}");
#endif
            Scene.SimulationMembers.Add(bot);
            Scene.SimulationManager.RegisterContactCallback(bot.BodyHandle, contactInfo => bot.ContactCallback(contactInfo, Scene.SimulationMembers));
            Scene.Bots.Add(bot);
        }
    }

    public override void Despawn()
    {
        for (int i = 0; i < Scene.Bots.Count; i++)
        {
            var bot = Scene.Bots[i];
            if (IsDead(bot))
            {
                Scene.Bots.RemoveAt(i);
                Scene.SimulationMembers.Remove(bot);
                if (!Scene.SimulationManager.UnregisterContactCallback(bot.BodyHandle))
                    throw new ApplicationException("Invalid simulation state!");
                bot.Dispose();
            }
        }
    }

    private Chunk GetRandomChunk(Random rand)
    {
        int index = rand.Next(Scene.Chunks.Count);
        return Scene.Chunks[index];
    }
}