using BepuPhysics;
using Common;
using OpenTK.Mathematics;
using Physics.Collisions;

namespace Character.Vehicles;
public class FourWheeledCar : ISimulationMember, IDisposable
{
    public SimpleCar SimpleCar { get; private init; }

    public IList<BodyHandle> BodyHandles { get; private init; }

    public BodyHandle BodyHandle => SimpleCar.BodyHandle;

    public int CurrentSphereId { get; set; }

    public RigidPose CarBodyPose => SimpleCar.CarBodyPose;

    private readonly FourWheeledCarModel _model;

    public FourWheeledCar(SimpleCar simpleCar, FourWheeledCarModel model, int currentSphereId)
    {
        SimpleCar = simpleCar;
        CurrentSphereId = currentSphereId;
        BodyHandles = new BodyHandle[5]
        {
            SimpleCar.BodyHandle,
            SimpleCar.BackLeftWheel.Wheel, SimpleCar.BackRightWheel.Wheel,
            SimpleCar.FrontLeftWheel.Wheel, SimpleCar.FrontRightWheel.Wheel
        };
        _model = model;
    }

    public void Render(Shader shader, float globalScale, float curve, Vector3 cameraPosition, Bodies simulationBodies)
        => _model.Render(this, shader, globalScale, curve, cameraPosition, simulationBodies);

    public void Update(Simulation simulation, float dt, float targetSteeringAngle, float targetSpeedFraction, bool zoom, bool brake)
    {
        SimpleCar.Update(simulation, dt, targetSteeringAngle, targetSpeedFraction, zoom, brake);
    }

    public void Dispose()
    {
        SimpleCar.Dispose();
    }
}
