using BepuPhysics;
using OpenTK.Mathematics;
using Physics.Collisions;
using Player;

namespace Hyper.Transporters;

// I hate this thing
public class NullTransporter : ITransporter
{
    public bool TryTeleportTo(int targetSphereId, ISimulationMember simulationMember, Simulation simulation, out Vector3 exitPoint)
    {
        exitPoint = default;
        return false;
    }

    public void UpdateCamera(int targetSphereId, Camera camera, Vector3 exitPoint)
    {
    }
}
