using BepuPhysics;
using Character.Vehicles;
using Hyper.PlayerData;
using OpenTK.Mathematics;
using Physics.Collisions;

namespace Hyper.Transporters;

// I hate this thing
internal class NullTransporter : ITransporter
{
    public bool TryTeleportCarTo(int targetSphereId, FourWheeledCar simulationMember, Simulation simulation, out Vector3 exitPoint)
    {
        exitPoint = default;
        return false;
    }

    public bool TryTeleportTo(int targetSphereId, ISimulationMember simulationMember, Simulation simulation, out Vector3 exitPoint)
    {
        exitPoint = default;
        return false;
    }

    public void UpdateCamera(int targetSphereId, Camera camera, Vector3 exitPoint)
    {
    }
}
