using System.Numerics;
using BepuPhysics;

namespace Physics.BoundingShapes;

/// <summary>
/// Represents a capsule-shaped bounding volume.
/// </summary>
public struct BoundingCapsule
{
    /// <summary>
    /// The position and orientation of the bounding capsule in 3D space.
    /// </summary>
    public RigidPose Pose;

    /// <summary>
    /// The radius of the capsule's hemispherical ends.
    /// </summary>
    public float Radius;

    /// <summary>
    /// Half of the length of the cylindrical part of the capsule.
    /// </summary>
    public float HalfLength;

    /// <summary>
    /// The ID of the sphere the bounding capsule is currently in
    /// </summary>
    public int SphereId;

    /// <summary>
    /// The color associated with the bounding capsule for visualization purposes.
    /// </summary>
    public Vector3 Color;
}
