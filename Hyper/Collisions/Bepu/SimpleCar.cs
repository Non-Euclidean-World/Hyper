// Copyright The Authors of bepuphysics2

// changes: added more creator methods

using System.Numerics;
using BepuPhysics;
using BepuPhysics.Collidables;
using BepuPhysics.Constraints;
using BepuUtilities;
using BepuUtilities.Memory;
using Hyper.Meshes;

namespace Hyper.Collisions.Bepu;
internal class SimpleCar
{
    public BodyHandle Body { get; private set; }
    public WheelHandles FrontLeftWheel { get; private set; }
    public WheelHandles FrontRightWheel { get; private set; }
    public WheelHandles BackLeftWheel { get; private set; }
    public WheelHandles BackRightWheel { get; private set; }

    public CarMesh Mesh { get => _mesh; }

    private Vector3 _suspensionDirection;
    private AngularHinge _hingeDescription;

    private readonly SimpleCarController _controller;

    private readonly CarMesh _mesh;

    private SimpleCar(SimpleCarController controller, CarMesh mesh)
    {
        _controller = controller;
        _mesh = mesh;
    }

    public void Steer(Simulation simulation, in WheelHandles wheel, float angle)
    {
        var steeredHinge = _hingeDescription;
        Matrix3x3.CreateFromAxisAngle(_suspensionDirection, -angle, out var rotation);
        Matrix3x3.Transform(_hingeDescription.LocalHingeAxisA, rotation, out steeredHinge.LocalHingeAxisA);
        simulation.Solver.ApplyDescription(wheel.Hinge, steeredHinge);
    }

    public static void SetSpeed(Simulation simulation, in WheelHandles wheel, float speed, float maximumForce)
    {
        simulation.Solver.ApplyDescription(wheel.Motor, new AngularAxisMotor
        {
            LocalAxisA = new Vector3(0, -1, 0),
            Settings = new MotorSettings(maximumForce, 1e-6f),
            TargetVelocity = speed
        });
    }

    private static WheelHandles CreateWheel(Simulation simulation, CollidableProperty<SimulationProperties> properties, in RigidPose bodyPose,
        TypedIndex wheelShape, BodyInertia wheelInertia, float wheelFriction, BodyHandle bodyHandle, ref SubgroupCollisionFilter bodyFilter, Vector3 bodyToWheelSuspension, Vector3 suspensionDirection, float suspensionLength,
        in AngularHinge hingeDescription, in SpringSettings suspensionSettings, Quaternion localWheelOrientation)
    {
        RigidPose wheelPose;
        RigidPose.Transform(bodyToWheelSuspension + suspensionDirection * suspensionLength, bodyPose, out wheelPose.Position);
        QuaternionEx.ConcatenateWithoutOverlap(localWheelOrientation, bodyPose.Orientation, out wheelPose.Orientation);
        WheelHandles handles;
        handles.Wheel = simulation.Bodies.Add(BodyDescription.CreateDynamic(wheelPose, wheelInertia, new(wheelShape, 0.5f), 0.01f));

        handles.SuspensionSpring = simulation.Solver.Add(bodyHandle, handles.Wheel, new LinearAxisServo
        {
            LocalPlaneNormal = suspensionDirection,
            TargetOffset = suspensionLength,
            LocalOffsetA = bodyToWheelSuspension,
            LocalOffsetB = default,
            ServoSettings = ServoSettings.Default,
            SpringSettings = suspensionSettings
        });
        handles.SuspensionTrack = simulation.Solver.Add(bodyHandle, handles.Wheel, new PointOnLineServo
        {
            LocalDirection = suspensionDirection,
            LocalOffsetA = bodyToWheelSuspension,
            LocalOffsetB = default,
            ServoSettings = ServoSettings.Default,
            SpringSettings = new SpringSettings(30, 1)
        });
        //We're treating braking and acceleration as the same thing. It is, after all, a *simple* car! Maybe it's electric or something.
        //It would be fairly easy to split brakes and drive motors into different motors.
        handles.Motor = simulation.Solver.Add(handles.Wheel, bodyHandle, new AngularAxisMotor
        {
            LocalAxisA = new Vector3(1, 0, 0),
            Settings = default,
            TargetVelocity = default
        });
        handles.Hinge = simulation.Solver.Add(bodyHandle, handles.Wheel, hingeDescription);
        //The demos SubgroupCollisionFilter is pretty simple and only tests one direction, so we make the non-colliding relationship symmetric.
        ref var wheelProperties = ref properties.Allocate(handles.Wheel);
        wheelProperties = new SimulationProperties { Filter = new SubgroupCollisionFilter(bodyHandle.Value, 1), Friction = wheelFriction };
        SubgroupCollisionFilter.DisableCollision(ref wheelProperties.Filter, ref bodyFilter);

        return handles;
    }

    private static SimpleCar Create(Simulation simulation, CollidableProperty<SimulationProperties> properties, in RigidPose pose,
        TypedIndex bodyShape, BodyInertia bodyInertia, float bodyFriction, TypedIndex wheelShape, BodyInertia wheelInertia, float wheelFriction,
        Vector3 bodyToFrontLeftSuspension, Vector3 bodyToFrontRightSuspension, Vector3 bodyToBackLeftSuspension, Vector3 bodyToBackRightSuspension,
        Vector3 suspensionDirection, float suspensionLength, in SpringSettings suspensionSettings, Quaternion localWheelOrientation,
        SimpleCarController controller, CarMesh mesh)
    {
        SimpleCar car = new SimpleCar(controller, mesh);
        car.Body = simulation.Bodies.Add(BodyDescription.CreateDynamic(pose, bodyInertia, new(bodyShape, 0.5f), 0.01f));
        ref var bodyProperties = ref properties.Allocate(car.Body);
        bodyProperties = new SimulationProperties { Friction = bodyFriction, Filter = new SubgroupCollisionFilter(car.Body.Value, 0) };
        QuaternionEx.TransformUnitY(localWheelOrientation, out var wheelAxis);
        car._hingeDescription = new AngularHinge
        {
            LocalHingeAxisA = wheelAxis,
            LocalHingeAxisB = new Vector3(0, 1, 0),
            SpringSettings = new SpringSettings(30, 1)
        };
        car._suspensionDirection = suspensionDirection;

        car.BackLeftWheel = CreateWheel(simulation, properties, pose, wheelShape, wheelInertia, wheelFriction, car.Body, ref bodyProperties.Filter, bodyToBackLeftSuspension, suspensionDirection, suspensionLength, car._hingeDescription, suspensionSettings, localWheelOrientation);
        car.BackRightWheel = CreateWheel(simulation, properties, pose, wheelShape, wheelInertia, wheelFriction, car.Body, ref bodyProperties.Filter, bodyToBackRightSuspension, suspensionDirection, suspensionLength, car._hingeDescription, suspensionSettings, localWheelOrientation);
        car.FrontLeftWheel = CreateWheel(simulation, properties, pose, wheelShape, wheelInertia, wheelFriction, car.Body, ref bodyProperties.Filter, bodyToFrontLeftSuspension, suspensionDirection, suspensionLength, car._hingeDescription, suspensionSettings, localWheelOrientation);
        car.FrontRightWheel = CreateWheel(simulation, properties, pose, wheelShape, wheelInertia, wheelFriction, car.Body, ref bodyProperties.Filter, bodyToFrontRightSuspension, suspensionDirection, suspensionLength, car._hingeDescription, suspensionSettings, localWheelOrientation);
        return car;
    }


    public static SimpleCar Create(Simulation simulation, BufferPool bufferPool, CollidableProperty<SimulationProperties> properties,
        in RigidPose initialPose,
        Box lowerPart, RigidPose lowerPartOrientation, float lowerPartWeight,
        Box upperPart, RigidPose upperPartOrientation, float upperPartWeight,
        float bodyFriction, float wheelFriction,
        float wheelRadius, float wheelWidth, float wheelMass,
        Vector3 bodyToFrontLeftSuspension, Vector3 bodyToFrontRightSuspension, Vector3 bodyToBackLeftSuspension, Vector3 bodyToBackRightSuspension,
        Vector3 suspensionDirection, float suspensionLength, in SpringSettings suspensionSettings, Quaternion localWheelOrientation,
        SimpleCarController controller, CarMesh mesh)
    {
        var builder = new CompoundBuilder(bufferPool, simulation.Shapes, 2);
        builder.Add(lowerPart, lowerPartOrientation, lowerPartWeight);
        builder.Add(upperPart, upperPartOrientation, upperPartWeight);
        builder.BuildDynamicCompound(out var children, out var bodyInertia, out _);
        builder.Dispose();
        var bodyShape = new Compound(children);
        var bodyShapeIndex = simulation.Shapes.Add(bodyShape);
        var wheelShape = new Cylinder(wheelRadius, wheelWidth);
        var wheelInertia = wheelShape.ComputeInertia(wheelMass);
        var wheelShapeIndex = simulation.Shapes.Add(wheelShape);

        return Create(simulation, properties, initialPose, bodyShapeIndex, bodyInertia, bodyFriction, wheelShapeIndex, wheelInertia,
            wheelFriction, bodyToFrontLeftSuspension, bodyToFrontRightSuspension,
            bodyToBackLeftSuspension, bodyToBackRightSuspension, suspensionDirection, suspensionLength,
            suspensionSettings, localWheelOrientation,
            controller, mesh);

    }

    // This car looks ridiculous but that's OK for the time being
    public static SimpleCar CreateStandardCar(Simulation simulation, BufferPool bufferPool, CollidableProperty<SimulationProperties> properties,
        in RigidPose initialPose)
    {
        const float x = 0.9f;
        const float y = -0.1f;
        const float frontZ = 1.7f;
        const float backZ = -1.7f;
        const float wheelRadius = 0.4f;
        const float wheelWidth = 0.18f;

        SimpleCarController controller = new SimpleCarController(forwardSpeed: 75, forwardForce: 6, zoomMultiplier: 2, backwardSpeed: 30, backwardForce: 4, idleForce: 0.25f, brakeForce: 7, steeringSpeed: 1.5f, maximumSteeringAngle: MathF.PI * 0.23f,
            wheelBaseLength: frontZ - backZ, wheelBaseWidth: x * 2, ackermanSteering: 1f);

        CarMesh mesh = new CarMesh(new OpenTK.Mathematics.Vector3(1.3f, 0.5f, 2.73f), wheelRadius, wheelWidth);

        return Create(simulation, bufferPool, properties, initialPose,
            lowerPart: new Box(1.3f, 0.5f, 2.73f),
            lowerPartOrientation: RigidPose.Identity,
            lowerPartWeight: 10,

            upperPart: new Box(1f, 0.4f, 0.5f),
            upperPartOrientation: new Vector3(0, 0.65f, -1f),
            upperPartWeight: 0.5f,

            bodyFriction: 0.5f, wheelFriction: 2f,
            wheelRadius: wheelRadius, wheelWidth: wheelWidth, wheelMass: 0.25f,

            new Vector3(x, y, frontZ), new Vector3(-x, y, frontZ), new Vector3(x, y, backZ), new Vector3(-x, y, backZ),
            suspensionDirection: new Vector3(0, -1, 0),
            suspensionLength: 0.25f,
            new SpringSettings(5f, 0.7f),
            localWheelOrientation: QuaternionEx.CreateFromAxisAngle(Vector3.UnitZ, MathF.PI * 0.5f),
            controller, mesh);
    }

    public void Update(Simulation simulation, float dt, float targetSteeringAngle, float targetSpeedFraction, bool zoom, bool brake)
    {
        _controller.Update(simulation, this, dt, targetSteeringAngle, targetSpeedFraction, zoom, brake);

        var carBody = new BodyReference(Body, simulation.Bodies);
        var rearLeftWheel = new BodyReference(BackLeftWheel.Wheel, simulation.Bodies);
        var rearRightWheel = new BodyReference(BackRightWheel.Wheel, simulation.Bodies);
        var frontLeftWheel = new BodyReference(FrontLeftWheel.Wheel, simulation.Bodies);
        var frontRightWheel = new BodyReference(FrontRightWheel.Wheel, simulation.Bodies);

        _mesh.Update(carBody.Pose, rearLeftWheel.Pose, rearRightWheel.Pose, frontLeftWheel.Pose, frontRightWheel.Pose);
    }

}