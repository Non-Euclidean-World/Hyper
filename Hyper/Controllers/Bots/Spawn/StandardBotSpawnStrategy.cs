using Chunks;
using Common;
using Hyper.GameEntities;
using OpenTK.Mathematics;
using Physics.TypingUtils;

namespace Hyper.Controllers.Bots.Spawn;

internal class StandardBotSpawnStrategy : AbstractBotSpawnStrategy
{
    private readonly float _despawnRadius;

    private readonly int _maxBots;

    private readonly float _minSpawnRadius;

    private readonly float _maxSpawnRadius;

    private static readonly TimeSpan EpsTime = new(0, 0, 0, 0, milliseconds: 200);

    public StandardBotSpawnStrategy(Scene scene, Settings settings) : base(scene, settings)
    {
        _maxBots = 10 * settings.RenderDistance * settings.RenderDistance * Chunk.Size / 32 * Chunk.Size / 32;
        _minSpawnRadius = Chunk.Size * settings.RenderDistance * 2f / 3;
        _maxSpawnRadius = Chunk.Size * settings.RenderDistance * 4f / 5;
        _despawnRadius = Chunk.Size * settings.RenderDistance;
    }

    public override void Spawn()
    {
        for (int i = 0; i < _maxBots - Scene.Bots.Count; i++)
        {
            var x = Rand.Next(0, 2) == 0 ? GetRandomFloat(-_maxSpawnRadius, -_minSpawnRadius) : GetRandomFloat(_minSpawnRadius, _maxSpawnRadius);
            var z = Rand.Next(0, 2) == 0 ? GetRandomFloat(-_maxSpawnRadius, -_minSpawnRadius) : GetRandomFloat(_minSpawnRadius, _maxSpawnRadius);
            var position = new Vector3(x + Scene.Camera.ReferencePointPosition.X, 0, z + Scene.Camera.ReferencePointPosition.Z);
            position.Y = GetSpawnHeight((int)position.X, (int)position.Z);
            var bot = new AstronautBot(Humanoid.CreatePhysicalCharacter(position, Scene.SimulationManager));
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
            if (IsTooFar(bot) || IsDead(bot))
            {
                Scene.Bots.RemoveAt(i);
                Scene.SimulationMembers.Remove(bot);
                if (!Scene.SimulationManager.UnregisterContactCallback(bot.BodyHandle))
                    throw new ApplicationException("Invalid simulation state!");
                bot.Dispose();
            }
        }
    }

    private float GetRandomFloat(float min, float max)
        => Rand.NextSingle() * (max - min) + min;

    private bool IsTooFar(Humanoid bot)
    {
        var distance = bot.PhysicalCharacter.Pose.Position - Conversions.ToNumericsVector(Scene.Camera.ReferencePointPosition);

        if (!(Math.Abs(distance.X) > _despawnRadius ||
              Math.Abs(distance.Y) > _despawnRadius ||
              Math.Abs(distance.Z) > _despawnRadius)) return false;
#if DEBUG
        Console.WriteLine($"Despawning bot {bot.BodyHandle}");
#endif
        return true;
    }

    private static bool IsDead(Humanoid bot)
    {
        return !bot.IsAlive && (DateTime.UtcNow - bot.DeathTime > EpsTime);
    }
}