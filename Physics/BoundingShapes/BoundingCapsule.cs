﻿using System.Numerics;
using BepuPhysics;

namespace Physics.BoundingShapes;
public struct BoundingCapsule
{
    public RigidPose Pose;
    public float Radius;
    public float HalfLength;
    public int SphereId;
    public Vector3 Color;
}
