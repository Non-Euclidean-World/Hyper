using System.Diagnostics.CodeAnalysis;
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

    /// <summary>
    /// Adds a simulation member and its body handles to the collection.
    /// </summary>
    /// <param name="member">The simulation member to add</param>
    public void Add(ISimulationMember member)
    {
        var bodyHandles = member.BodyHandles;
        foreach (var handle in bodyHandles)
        {
            _simulationMembers.Add(handle, member);
        }
    }

    /// <summary>
    /// Gets a simulation member with a given body handle.
    /// </summary>
    /// <param name="handle">Body handle to find the simulation member by</param>
    /// <param name="member">Simulation member that uses the body handle or null if it couldn't be found</param>
    /// <returns>True if there is a simulation member using the given body handle, false otherwise</returns>
    public bool TryGetByHandle(BodyHandle handle, [MaybeNullWhen(false)] out ISimulationMember member)
    {
        return _simulationMembers.TryGetValue(handle, out member);
    }

    /// <summary>
    /// Removes all body handles associated with a simulation member.
    /// </summary>
    /// <param name="member">The simulation member whose body handles should be removed</param>
    /// <returns>True if all body handles were removed successfully, false otherwise</returns>
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

    /// <summary>
    /// Checks if there is a simulation member with a given handle.
    /// </summary>
    /// <param name="handle">Handle to find the simulation member by</param>
    /// <returns>True if there is a simulation member with the given handle, false otherwise</returns>
    public bool Contains(BodyHandle handle)
    {
        return _simulationMembers.ContainsKey(handle);
    }

    /// <summary>
    /// Gets or sets a simulation member that uses the given body handle.
    /// </summary>
    /// <param name="handle">The body handle to get or set the associated simulation member</param>
    /// <returns>The simulation member associated with the provided body handle</returns>
    public ISimulationMember this[BodyHandle handle]
    {
        get { return _simulationMembers[handle]; }
        set { _simulationMembers[handle] = value; }
    }
}
