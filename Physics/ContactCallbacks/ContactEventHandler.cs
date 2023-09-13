using System.Numerics;
using BepuPhysics;
using BepuPhysics.Collidables;
using BepuPhysics.CollisionDetection;

namespace Physics.ContactCallbacks;
internal class ContactEventHandler : IContactEventHandler
{
    private readonly Dictionary<BodyHandle, Action<ContactInfo>> _contactCallbacks;
    private readonly Simulation _simulation;
#if DEBUG
    private ulong _counter;
#endif
    public ContactEventHandler(Simulation simulation, Dictionary<BodyHandle, Action<ContactInfo>> contactCallbacks)
    {
        _simulation = simulation;
        _contactCallbacks = contactCallbacks;
    }

    public void OnContactAdded<TManifold>(CollidableReference eventSource, CollidablePair pair, ref TManifold contactManifold,
                Vector3 contactOffset, Vector3 contactNormal, float depth, int featureId, int contactIndex, int workerIndex) where TManifold : unmanaged, IContactManifold<TManifold>
    {
        var collisionLocation = contactOffset + (pair.A.Mobility == CollidableMobility.Static ?
                        new StaticReference(pair.A.StaticHandle, _simulation.Statics).Pose.Position :
                        new BodyReference(pair.A.BodyHandle, _simulation.Bodies).Pose.Position);

        ContactInfo collisionInfo = new ContactInfo
        {
            CollidablePair = pair,
            ContactLocation = collisionLocation,
#if DEBUG
            ContactNumber = _counter++
#endif
        };

        if (pair.A.Mobility == CollidableMobility.Dynamic
            && _contactCallbacks.TryGetValue(pair.A.BodyHandle, out var callbackA))
            callbackA(collisionInfo);
        if (pair.B.Mobility == CollidableMobility.Dynamic
            && _contactCallbacks.TryGetValue(pair.B.BodyHandle, out var callbackB))
            callbackB(collisionInfo);
    }
}
