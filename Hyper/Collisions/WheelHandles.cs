// Copyright The Authors of bepuphysics2

using BepuPhysics;

namespace Hyper.Collisions;
struct WheelHandles
{
    public BodyHandle Wheel;
    public ConstraintHandle SuspensionSpring;
    public ConstraintHandle SuspensionTrack;
    public ConstraintHandle Hinge;
    public ConstraintHandle Motor;
}
