using System.Numerics;
using BepuPhysics;

namespace Physics.BoundingShapes;
public struct BoundingBox
{
    public RigidPose Pose;
    public float HalfLength;
    public float HalfWidth;
    public float HalfHeight;
    public int SphereId;
    public Vector3 Color;
}
