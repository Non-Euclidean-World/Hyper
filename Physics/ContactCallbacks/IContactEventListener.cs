using BepuPhysics;
using Physics.Collisions;

namespace Physics.ContactCallbacks;
public interface IContactEventListener
{
    /// <summary>
    /// Action taken by an object when it comes into contact with another object
    /// </summary>
    /// <param name="collisionInfo">Information about the contact</param>
    /// <param name="simulationMembers">Objects present in the simulation</param>
    public void ContactCallback(ContactInfo collisionInfo, Dictionary<BodyHandle, ISimulationMember> simulationMembers);
}
