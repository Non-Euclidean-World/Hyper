using System.Numerics;
using BepuPhysics;

namespace Physics.BoundingShapes;

/// <summary>
/// Represents a cylinder-shaped bounding volume in 3D space.
/// </summary>
public struct BoundingCylinder
{
    /// <summary>
    /// The position and orientation of the bounding cylinder in 3D space.
    /// </summary>
    public RigidPose Pose;

    /// <summary>
    /// The radius of the cylinder.
    /// </summary>
    public float Radius;

    /// <summary>
    /// Half of the length along the central axis of the cylinder.
    /// </summary>
    public float HalfLength;

    /// <summary>
    /// The ID of the sphere the bounding cylinder is currently in
    /// </summary>
    public int SphereId;

    /// <summary>
    /// The color associated with the bounding cylinder for visualization purposes.
    /// </summary>
    public Vector3 Color;
}
