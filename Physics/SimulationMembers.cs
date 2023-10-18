using BepuPhysics;
using Physics.Collisions;

namespace Physics;

/// <summary>
/// A collection that associates body handles with game objects.
/// This is a utility class for look-up only. Adding or removing doesn't influence the physics engine.
/// </summary>
public class SimulationMembers
{
    private readonly Dictionary<BodyHandle, ISimulationMember> _simulationMembers = new();

    public void Add(ISimulationMember member)
    {
        var bodyHandles = member.BodyHandles;
        foreach (var handle in bodyHandles)
        {
            _simulationMembers.Add(handle, member);
        }
    }

    public bool TryGetByHandle(BodyHandle handle, out ISimulationMember? member)
    {
        return _simulationMembers.TryGetValue(handle, out member);
    }

    public bool Remove(ISimulationMember member)
    {
        var bodyHandles = member.BodyHandles;
        bool notFound = false;
        foreach (var handle in bodyHandles)
        {
            notFound = notFound || !_simulationMembers.Remove(handle);
        }

        return notFound;
    }

    public bool Contains(BodyHandle handle)
    {
        return _simulationMembers.ContainsKey(handle);
    }

    public ISimulationMember this[BodyHandle handle]
    {
        get { return _simulationMembers[handle]; }
        set { _simulationMembers[handle] = value; }
    }
}
