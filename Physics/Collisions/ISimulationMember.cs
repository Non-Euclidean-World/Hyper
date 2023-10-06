﻿using BepuPhysics;

namespace Physics.Collisions;
public interface ISimulationMember
{
    public BodyHandle BodyHandle { get; }

    public int CurrentSphereId { get; set; }
}
