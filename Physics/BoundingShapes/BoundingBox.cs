using System.Numerics;
using BepuPhysics;

namespace Physics.BoundingShapes;

/// <summary>
/// Represents a bounding box.
/// </summary>
public struct BoundingBox
{
    /// <summary>
    /// The position and orientation of the bounding box in 3D space.
    /// </summary>
    public RigidPose Pose;

    /// <summary>
    /// Half of the length of the bounding box along its local x-axis.
    /// </summary>
    public float HalfLength;

    /// <summary>
    /// Half of the width of the bounding box along its local z-axis.
    /// </summary>
    public float HalfWidth;

    /// <summary>
    /// Half of the height of the bounding box along its local y-axis.
    /// </summary>
    public float HalfHeight;

    /// <summary>
    /// The ID of the sphere the bounding box is currently in
    /// </summary>
    public int SphereId;

    /// <summary>
    /// The color associated with the bounding box for visualization purposes.
    /// </summary>
    public Vector3 Color;
}
