using Character.Vehicles;
using Common;
using OpenTK.Mathematics;
using Physics.Collisions;
using Physics.TypingUtils;

namespace Hyper.Controllers.Bots.Spawn;
internal class CarSpawnStrategy : AbstractBotSpawnStrategy
{
    public CarSpawnStrategy(Scene scene, Settings settings) : base(scene, settings)
    {
    }

    public override void Despawn()
    {
        throw new NotImplementedException();
    }

    public override void Spawn()
    {
        const int x = 5;
        const int z = 11;
        const float heightOffset = 2f;
        float y = SpawnUtils.GetSpawnHeight(x, z, Scene) + heightOffset;
        var carInitialPosition = new Vector3(x, y, z);

        var car = new SpaceMustang(SimpleCar.CreateStandardCar(Scene.SimulationManager.Simulation, Scene.SimulationManager.BufferPool, Scene.SimulationManager.Properties,
                Conversions.ToNumericsVector(carInitialPosition)), currentSphereId: 0);

        Scene.FreeCars.Add(car);
        Scene.SimulationMembers.Add(car);
    }
}
