﻿using BepuPhysics;
using Character.Vehicles;
using Hyper.PlayerData;
using OpenTK.Mathematics;
using Physics.Collisions;

namespace Hyper.Transporters;

// I hate this thing
internal class NullTransporter : ITransporter
{
    public bool TryTeleportCarTo(int targetSphereId, SimpleCar simulationMember, Simulation simulation, out Vector3 exitPoint)
    {
        throw new NotImplementedException();
    }

    public bool TryTeleportTo(int targetSphereId, ISimulationMember simulationMember, Simulation simulation, out Vector3 exitPoint)
    {
        throw new NotImplementedException();
    }

    public void UpdateCamera(int targetSphereId, Camera camera, Vector3 exitPoint)
    {
    }
}
