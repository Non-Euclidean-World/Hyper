using BepuPhysics;
using OpenTK.Mathematics;
using Physics.Collisions;
using Physics.TypingUtils;
using Player;
using Numerics = System.Numerics;

namespace Hyper;
internal class SphericalTransporter
{
    private readonly float _radius;
    private readonly float _globalScale;
    private readonly Vector3i[] _sphereCenters;

    public SphericalTransporter(float globalScale, Vector3i[] sphereCenters)
    {
        _globalScale = globalScale;
        _radius = MathF.PI / 2 / _globalScale;
        _sphereCenters = sphereCenters;
    }

    public bool TryTeleportTo(int targetSphereId, ISimulationMember simulationMember, Simulation simulation, out Vector3 exitPoint)
    {
        int currentSphereId = 1 - targetSphereId;
        var bodyReference = new BodyReference(simulationMember.BodyHandle, simulation.Bodies);

        Vector3 bodyPosition = Conversions.ToOpenTKVector(bodyReference.Pose.Position);
        Vector3 bodyPositionXZ = new Vector3(bodyPosition.X, 0, bodyPosition.Z);

        if (Vector3.Distance(bodyPositionXZ, _sphereCenters[currentSphereId]) > _radius)
        {
            Vector3 posAfterTeleportXZ = _sphereCenters[targetSphereId] + 0.97f * FlipXZ(bodyPositionXZ - _sphereCenters[currentSphereId]);
            Vector3 posAfterTeleport = new Vector3(posAfterTeleportXZ.X, bodyPosition.Y, posAfterTeleportXZ.Z);

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
        int currentSphereId = 1 - targetSphereId;
        camera.Sphere = targetSphereId;
        camera.SphereCenter = _sphereCenters[targetSphereId];

        if (targetSphereId == 1)
            camera.FrontTransform = (Vector3 f) => ReflectFront(f, _sphereCenters[currentSphereId], exitPoint);
        else
            camera.FrontTransform = Camera.IdentityTransform;

        camera.UpdateVectors();
    }

    private static Vector3 FlipXZ(Vector3 v) => new(-v.X, v.Y, -v.Z);

    private static Numerics.Vector3 ReflectVelocity(Numerics.Vector3 velocity, Vector3 sphereCenter, Vector3 teleportPoint)
    {
        Vector3 n = Vector3.Normalize(Vector3.Cross(teleportPoint - sphereCenter, Vector3.UnitY));
        return Conversions.ToNumericsVector(Common.GeomPorting.ReflectVector(Conversions.ToOpenTKVector(velocity), n));
    }

    private static Vector3 ReflectFront(Vector3 front, Vector3 sphereCenter, Vector3 teleportPoint)
    {
        Vector3 n = Vector3.Normalize(Vector3.Cross(teleportPoint - sphereCenter, Vector3.UnitY));
        Vector3 n90 = new Vector3(-n.Z, 0, n.X); // 90 degrees flip
        return Common.GeomPorting.ReflectVector(front, n90);
    }
}
