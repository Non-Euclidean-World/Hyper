using Common;
using Hyper.GameEntities;

namespace Hyper.Controllers.Bots.Spawn;

internal abstract class AbstractBotSpawnStrategy
{
    protected readonly Scene Scene;

    protected readonly Random Rand;

    protected static readonly TimeSpan ProclaimedDeadTime = new(0, 0, 0, 0, milliseconds: 200);

    protected AbstractBotSpawnStrategy(Scene scene, Settings settings)
    {
        Scene = scene;
        Rand = new Random(settings.Seed);
    }

    public abstract void Spawn();

    public abstract void Despawn();

    protected static bool IsDead(Humanoid bot)
    {
        return !bot.IsAlive && (DateTime.UtcNow - bot.DeathTime > ProclaimedDeadTime);
    }
}