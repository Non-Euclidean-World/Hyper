using BepuPhysics;

namespace Physics.Collisions;
public interface ISimulationMember
{
    /// <summary>
    /// List of all body handles used by the simulation member
    /// </summary>
    public IList<BodyHandle> BodyHandles { get; }

    public int CurrentSphereId { get; set; }
}
