using Character.GameEntities;
using Chunks;
using Common;
using OpenTK.Mathematics;
using Physics.TypingUtils;

namespace Hyper.Controllers.Bots.Spawn;

internal class StandardBotSpawnStrategy : AbstractBotSpawnStrategy
{
    private readonly float _despawnRadius;

    private readonly int _maxBots;

    private readonly int _minSpawnRadius;

    private readonly int _maxSpawnRadius;

    public StandardBotSpawnStrategy(Scene scene, Settings settings) : base(scene, settings)
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
            var x = Rand.Next(0, 2) == 0 ? Rand.Next(-_maxSpawnRadius, -_minSpawnRadius) : Rand.Next(_minSpawnRadius, _maxSpawnRadius);
            var z = Rand.Next(0, 2) == 0 ? Rand.Next(-_maxSpawnRadius, -_minSpawnRadius) : Rand.Next(_minSpawnRadius, _maxSpawnRadius);
            var position = new Vector3(x + Scene.Camera.ReferencePointPosition.X, 0, z + Scene.Camera.ReferencePointPosition.Z);
            position.Y = GetSpawnHeight((int)position.X, (int)position.Z);
            var bot = new Cowboy(Humanoid.CreatePhysicalCharacter(position, Scene.SimulationManager));
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
        Scene.Bots.RemoveAll(bot =>
        {
            var distance = bot.PhysicalCharacter.Pose.Position - Conversions.ToNumericsVector(Scene.Camera.ReferencePointPosition);

            if (!(Math.Abs(distance.X) > _despawnRadius ||
                  Math.Abs(distance.Y) > _despawnRadius ||
                  Math.Abs(distance.Z) > _despawnRadius)) return false;
#if DEBUG
            Console.WriteLine($"Despawning bot {bot.BodyHandle}");
#endif
            bot.Dispose();
            Scene.SimulationMembers.Remove(bot);
            return true;
        });
    }
}