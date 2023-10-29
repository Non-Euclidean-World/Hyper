using Character.GameEntities;
using Chunks;
using Common;
using OpenTK.Mathematics;

namespace Hyper.Controllers.Bots.Spawn;

internal class SphericalBotSpawnStrategy : AbstractBotSpawnStrategy
{
    private const int MaxBots = 20;

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
            position.Y = GetSpawnHeight((int)position.X, (int)position.Z);
            var bot = new AstronautBot(Humanoid.CreatePhysicalCharacter(position, Scene.SimulationManager))
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
        // Nothing to do here.
    }

    private Chunk GetRandomChunk(Random rand)
    {
        int index = rand.Next(Scene.Chunks.Count);
        return Scene.Chunks[index];
    }
}