using Common;
using OpenTK.Mathematics;

namespace Hyper.Controllers.Bots.Spawn;
internal class PlayerSpawnStrategy : AbstractBotSpawnStrategy
{
    public PlayerSpawnStrategy(Scene scene, Settings settings) : base(scene, settings)
    {
    }

    public override void Despawn()
    {
        throw new NotImplementedException();
    }

    public override void Spawn()
    {
        throw new NotImplementedException();
    }

    public Vector3 GetSpawnLocation()
    {
        return new Vector3(0, GetSpawnHeight(0, 0), 0);
    }
}
