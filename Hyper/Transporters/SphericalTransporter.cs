using BepuPhysics;
using OpenTK.Mathematics;
using Physics.Collisions;
using Physics.TypingUtils;
using Player;
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
        var bodyReference = new BodyReference(simulationMember.BodyHandle, simulation.Bodies);

        var bodyPosition = Conversions.ToOpenTKVector(bodyReference.Pose.Position);
        var bodyPositionXZ = new Vector3(bodyPosition.X, 0, bodyPosition.Z);

        if (Vector3.Distance(bodyPositionXZ, _sphereCenters[currentSphereId]) > _radius)
        {
            var posAfterTeleportXZ = _sphereCenters[targetSphereId] + 0.99f * FlipXZ(bodyPositionXZ - _sphereCenters[currentSphereId]);
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
