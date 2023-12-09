using BepuPhysics;

namespace Physics.Collisions;
public interface ISimulationMember
{
    /// <summary>
    /// List of all body handles used by the simulation member.
    /// </summary>
    public IList<BodyHandle> BodyHandles { get; }

    /// <summary>
    /// ID of the hypersphere the simulation member is currently in.
    /// Attains two values: 0 for the upper hypersphere, 1 for the lower hypersphere.
    /// </summary>
    public int CurrentSphereId { get; set; }
}
