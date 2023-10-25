// Copyright The Authors of bepuphysics2

// changes: added BodyToWheelSuspension

using System.Numerics;
using BepuPhysics;

namespace Physics.Collisions;
public struct WheelHandles
{
    public BodyHandle Wheel;
    public ConstraintHandle SuspensionSpring;
    public ConstraintHandle SuspensionTrack;
    public ConstraintHandle Hinge;
    public ConstraintHandle Motor;
    public Vector3 BodyToWheelSuspension;
}
