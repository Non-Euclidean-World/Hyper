using Character.GameEntities;
using Chunks;
using OpenTK.Mathematics;

namespace Hyper.Controllers.Bots.Spawn;

internal class SphericalAbstractBotSpawnStrategy : AbstractBotSpawnStrategy
{
    private const int MaxBots = 20;

    private const int DistanceFromChunkCenter = Chunk.Size / 4;

    public SphericalAbstractBotSpawnStrategy(Scene scene) : base(scene) { }
    
    public override void Spawn()
    {
        while (Scene.Bots.Count < MaxBots)
        {
            var rand = new Random();
            var chunk = GetRandomChunk(rand);
            var x = Chunk.Size / 2 + rand.Next(-DistanceFromChunkCenter, DistanceFromChunkCenter);
            var z = Chunk.Size / 2 + rand.Next(-DistanceFromChunkCenter, DistanceFromChunkCenter);
            var position = chunk.Position + new Vector3(x, 0, z);
            position.Y = GetSpawnHeight((int)position.X, (int)position.Z);
            var bot = new Cowboy(Humanoid.CreatePhysicalCharacter(position, Scene.SimulationManager))
            {
                CurrentSphereId = chunk.Sphere
            };
#if DEBUG
            Console.WriteLine($"Spawning bot {bot.BodyHandle}");
#endif
            Scene.SimulationMembers.Add(bot.BodyHandle, bot);
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