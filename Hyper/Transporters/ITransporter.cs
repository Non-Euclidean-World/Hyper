﻿using BepuPhysics;
using OpenTK.Mathematics;
using Physics.Collisions;
using Player;

namespace Hyper.Transporters;
internal interface ITransporter
{
    bool TryTeleportTo(int targetSphereId, ISimulationMember simulationMember, Simulation simulation, out Vector3 exitPoint);

    void UpdateCamera(int targetSphereId, Camera camera, Vector3 exitPoint);
}
