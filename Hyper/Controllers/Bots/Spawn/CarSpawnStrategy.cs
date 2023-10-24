using Character.Vehicles;
using Common;
using OpenTK.Mathematics;
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
        float y = GetSpawnHeight(x, z);
        var carInitialPosition = new Vector3(x, y, z);

        var car = SimpleCar.CreateStandardCar(Scene.SimulationManager.Simulation, Scene.SimulationManager.BufferPool, Scene.SimulationManager.Properties,
                Conversions.ToNumericsVector(carInitialPosition));

        Scene.FreeCars.Add(car);
        Scene.SimulationMembers.Add(car);
    }
}
