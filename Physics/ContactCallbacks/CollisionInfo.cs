using System.Numerics;
using BepuPhysics.CollisionDetection;

namespace Physics.ContactCallbacks;

/// <summary>
/// Information describing the collision
/// </summary>
public struct ContactInfo
{
    /// <summary>
    /// Colliding bodies
    /// </summary>
    public CollidablePair CollidablePair;

    /// <summary>
    /// Location where the collision took place
    /// </summary>
    public Vector3 ContactLocation;

#if DEBUG
    public ulong ContactNumber;
#endif
}
