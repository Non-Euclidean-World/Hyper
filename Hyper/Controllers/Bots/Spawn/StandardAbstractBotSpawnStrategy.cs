using Character.GameEntities;
using Chunks;
using Common;
using OpenTK.Mathematics;

namespace Hyper.Controllers.Bots.Spawn;

internal class StandardAbstractBotSpawnStrategy : AbstractBotSpawnStrategy
{
    private readonly float _despawnRadius;
    
    private readonly int _maxBots;

    private readonly int _minSpawnRadius;
    
    private readonly int _maxSpawnRadius;
    
    public StandardAbstractBotSpawnStrategy(Scene scene, Settings settings) : base(scene)
    {
        _maxBots = 10 * settings.RenderDistance * settings.RenderDistance;
        _minSpawnRadius = Chunk.Size * settings.RenderDistance / 3;
        _maxSpawnRadius = Chunk.Size * settings.RenderDistance * 2 / 3;
        _despawnRadius = Chunk.Size * settings.RenderDistance;
    }
        
    public override void Spawn()
    {
        for (int i = 0; i < _maxBots - Scene.Bots.Count; i++)
        {
            var rand = new Random();
            var x = rand.Next(0, 2) == 0 ? rand.Next(-_maxSpawnRadius, -_minSpawnRadius) : rand.Next(_minSpawnRadius, _maxSpawnRadius);
            var z = rand.Next(0, 2) == 0 ? rand.Next(-_maxSpawnRadius, -_minSpawnRadius) : rand.Next(_minSpawnRadius, _maxSpawnRadius);
            var position = new Vector3(x + Scene.Player.PhysicalCharacter.Pose.Position.X, 0, z + Scene.Player.PhysicalCharacter.Pose.Position.Z);
            position.Y = GetSpawnHeight((int)position.X, (int)position.Z);
            var bot = new Cowboy(Humanoid.CreatePhysicalCharacter(position, Scene.SimulationManager));
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
        Scene.Bots.RemoveAll(bot =>
        {
            var distance = (bot.PhysicalCharacter.Pose.Position - Scene.Player.PhysicalCharacter.Pose.Position);

            if (!(Math.Abs(distance.X) > _despawnRadius || 
                  Math.Abs(distance.Z) > _despawnRadius || 
                  Math.Abs(distance.Z) > _despawnRadius)) return false;
#if DEBUG
            Console.WriteLine($"Despawning bot {bot.BodyHandle}");
#endif
            bot.Dispose();
            Scene.SimulationMembers.Remove(bot.BodyHandle);
            return true;
        });
    }
}