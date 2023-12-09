using BepuPhysics;
using BepuUtilities;
using Common;
using Common.Meshes;
using OpenTK.Mathematics;
using Physics.Collisions;
using Physics.TypingUtils;

namespace Character.Vehicles;
/// <summary>
/// A car with four wheels.
/// </summary>
public class FourWheeledCar : ISimulationMember, IDisposable
{
    public SimpleCar SimpleCar { get; private init; }

    public IList<BodyHandle> BodyHandles { get; private init; }

    public BodyHandle BodyHandle => SimpleCar.BodyHandle;

    public int CurrentSphereId { get; set; }

    public RigidPose CarBodyPose => SimpleCar.CarBodyPose;

    /// <summary>
    /// Front and rear lights
    /// </summary>
    public List<FlashLight> Lights { get; private init; }

    private readonly FourWheeledCarModel _model;

    protected FourWheeledCar(SimpleCar simpleCar, FourWheeledCarModel model, int currentSphereId)
    {
        SimpleCar = simpleCar;
        CurrentSphereId = currentSphereId;
        BodyHandles = new BodyHandle[5]
        {
            SimpleCar.BodyHandle,
            SimpleCar.BackLeftWheel.Wheel, SimpleCar.BackRightWheel.Wheel,
            SimpleCar.FrontLeftWheel.Wheel, SimpleCar.FrontRightWheel.Wheel
        };
        Lights = new List<FlashLight>
        {
            FlashLight.CreateCarLight(Vector3.One, currentSphereId),
            FlashLight.CreateCarLight(Vector3.One, currentSphereId),
            FlashLight.CreateCarRearLight(new Vector3(1, 0, 0), currentSphereId),
            FlashLight.CreateCarRearLight(new Vector3(1, 0, 0), currentSphereId),
        };
        _model = model;
    }

    /// <summary>
    /// Renders the car.
    /// </summary>
    /// <param name="shader">The shader used for rendering.</param>
    /// <param name="globalScale">The scale of the scene.</param>
    /// <param name="curve">The curvature of the scene.</param>
    /// <param name="cameraPosition">The camera position in the scene.</param>
    /// <param name="simulationBodies">The bodies in the physics simulation.</param>
    public void Render(Shader shader, float globalScale, float curve, Vector3 cameraPosition, Bodies simulationBodies)
        => _model.Render(this, shader, globalScale, curve, cameraPosition, simulationBodies);

    /// <summary>
    /// Updates the car.
    /// </summary>
    /// <param name="simulation">The physics simulation.</param>
    /// <param name="dt">The time since last update.</param>
    /// <param name="targetSteeringAngle">The target steering angle.</param>
    /// <param name="targetSpeedFraction">The target speed fraction.</param>
    /// <param name="zoom">Whether the car is in turbo (moves faster than usual).</param>
    /// <param name="brake">If the car is breaking.</param>
    public void Update(Simulation simulation, float dt, float targetSteeringAngle, float targetSpeedFraction, bool zoom, bool brake)
    {
        SimpleCar.Update(simulation, dt, targetSteeringAngle, targetSpeedFraction, zoom, brake);

        for (var i = 0; i < Lights.Count; i++)
        {
            var light = Lights[i];
            QuaternionEx.Transform(SimpleCar.Lights[i], CarBodyPose.Orientation, out var lightPos);
            light.Position = Conversions.ToOpenTKVector(CarBodyPose.Position + lightPos);
            QuaternionEx.TransformUnitZ(CarBodyPose.Orientation, out var direction);
            light.Direction = Conversions.ToOpenTKVector(i <= 1 ? direction : -direction);
        }
    }

    /// <summary>
    /// Disposes of the car.
    /// </summary>
    public void Dispose()
    {
        SimpleCar.Dispose();
    }
}
