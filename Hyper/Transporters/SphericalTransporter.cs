using BepuPhysics;
using Character.Vehicles;
using Hyper.PlayerData;
using OpenTK.Mathematics;
using Physics.Collisions;
using Physics.TypingUtils;
using Numerics = System.Numerics;

namespace Hyper.Transporters;
internal class SphericalTransporter : ITransporter
{
    private readonly float _radius;
    private readonly Vector3i[] _sphereCenters;

    public SphericalTransporter(float cutoffRadius, Vector3i[] sphereCenters)
    {
        _radius = cutoffRadius;
        _sphereCenters = sphereCenters;
    }

    public bool TryTeleportTo(int targetSphereId, ISimulationMember simulationMember, Simulation simulation, out Vector3 exitPoint)
    {
        var currentSphereId = 1 - targetSphereId;
        var bodyReference = new BodyReference(simulationMember.BodyHandles[0], simulation.Bodies);

        var bodyPosition = Conversions.ToOpenTKVector(bodyReference.Pose.Position);
        var bodyPositionXZ = new Vector3(bodyPosition.X, 0, bodyPosition.Z);

        if (Vector3.Distance(bodyPositionXZ, _sphereCenters[currentSphereId]) > _radius)
        {
            var posAfterTeleportXZ = _sphereCenters[targetSphereId] + FlipXZ(bodyPositionXZ - _sphereCenters[currentSphereId]);
            var posAfterTeleport = new Vector3(posAfterTeleportXZ.X, bodyPosition.Y, posAfterTeleportXZ.Z);

            bodyReference.Pose = new RigidPose(Conversions.ToNumericsVector(posAfterTeleport), bodyReference.Pose.Orientation);
            bodyReference.Velocity.Linear = ReflectVelocity(bodyReference.Velocity.Linear, _sphereCenters[currentSphereId], bodyPositionXZ);

            simulationMember.CurrentSphereId = targetSphereId;
            exitPoint = bodyPositionXZ;
            return true;
        }

        exitPoint = default;
        return false;
    }

    public bool TryTeleportCarTo(int targetSphereId, SimpleCar car, Simulation simulation, out Vector3 exitPoint)
    {
        var currentSphereId = 1 - targetSphereId;
        var bodyReference = new BodyReference(car.BodyHandle, simulation.Bodies);
        var frontLeftWheelReference = new BodyReference(car.FrontLeftWheel.Wheel, simulation.Bodies);
        var frontRightWheelReference = new BodyReference(car.FrontRightWheel.Wheel, simulation.Bodies);
        var backLeftWheelReference = new BodyReference(car.BackLeftWheel.Wheel, simulation.Bodies);
        var backRightWheelReference = new BodyReference(car.BackRightWheel.Wheel, simulation.Bodies);

        var bodyPosition = Conversions.ToOpenTKVector(bodyReference.Pose.Position);
        var bodyPositionXZ = new Vector3(bodyPosition.X, 0, bodyPosition.Z);

        if (Vector3.Distance(bodyPositionXZ, _sphereCenters[currentSphereId]) > 0.95 * _radius)
        {
            var posAfterTeleportXZ = _sphereCenters[targetSphereId] + 0.9f * FlipXZ(bodyPositionXZ - _sphereCenters[currentSphereId]);
            var posAfterTeleport = new Vector3(posAfterTeleportXZ.X, 1.1f * bodyPosition.Y, posAfterTeleportXZ.Z);

            bodyReference.Pose = new RigidPose(Conversions.ToNumericsVector(posAfterTeleport), bodyReference.Pose.Orientation);

            TransformWheelPosition(posAfterTeleport, car.FrontLeftWheel.BodyToWheelSuspension, ref frontLeftWheelReference);
            TransformWheelPosition(posAfterTeleport, car.FrontRightWheel.BodyToWheelSuspension, ref frontRightWheelReference);
            TransformWheelPosition(posAfterTeleport, car.BackLeftWheel.BodyToWheelSuspension, ref backLeftWheelReference);
            TransformWheelPosition(posAfterTeleport, car.BackRightWheel.BodyToWheelSuspension, ref backRightWheelReference);

            car.CurrentSphereId = targetSphereId;
            exitPoint = bodyPositionXZ;
            return true;
        }
        exitPoint = default;
        return false;
    }

    private static void TransformWheelPosition(Vector3 bodyPosAfterTeleport, Numerics.Vector3 bodyToWheelSuspension, ref BodyReference wheelReference)
    {
        wheelReference.Pose = new RigidPose(Conversions.ToNumericsVector(bodyPosAfterTeleport) + bodyToWheelSuspension, wheelReference.Pose.Orientation);
    }

    public void UpdateCamera(int targetSphereId, Camera camera, Vector3 exitPoint)
    {
        var currentSphereId = 1 - targetSphereId;
        camera.Sphere = targetSphereId;
        camera.SphereCenter = _sphereCenters[targetSphereId];

        if (targetSphereId == 1)
            camera.FrontTransform = (f) => ReflectFront(f, _sphereCenters[currentSphereId], exitPoint);
        else
            camera.FrontTransform = Camera.IdentityTransform;

        camera.UpdateVectors();
    }

    private static Vector3 FlipXZ(Vector3 v) => new(-v.X, v.Y, -v.Z);

    private static Numerics.Vector3 ReflectVelocity(Numerics.Vector3 velocity, Vector3 sphereCenter, Vector3 teleportPoint)
    {
        var n = Vector3.Normalize(Vector3.Cross(teleportPoint - sphereCenter, Vector3.UnitY));
        return Conversions.ToNumericsVector(Common.GeomPorting.ReflectVector(Conversions.ToOpenTKVector(velocity), n));
    }

    private static Vector3 ReflectFront(Vector3 front, Vector3 sphereCenter, Vector3 teleportPoint)
    {
        var n = Vector3.Normalize(Vector3.Cross(teleportPoint - sphereCenter, Vector3.UnitY));
        var n90 = new Vector3(-n.Z, 0, n.X); // 90 degrees flip
        return Common.GeomPorting.ReflectVector(front, n90);
    }
}
