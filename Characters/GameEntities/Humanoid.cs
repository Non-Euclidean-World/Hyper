using BepuPhysics;
using BepuPhysics.Collidables;
using Character.Characters;
using Character.Characters.Cowboy;
using Common;
using OpenTK.Mathematics;
using Physics.Collisions;
using Physics.ContactCallbacks;
using Physics.TypingUtils;

namespace Character.GameEntities;
public abstract class Humanoid : ISimulationMember, IContactEventListener
{
    public Model Character { get; init; }

    public PhysicalCharacter PhysicalCharacter { get; init; }

    public BodyHandle BodyHandle => PhysicalCharacter.BodyHandle;

    protected Vector3 ViewDirection;

    protected DateTime LastContactTime = DateTime.MinValue;

    protected BodyHandle? LastContactBody;

    protected static readonly TimeSpan EpsTime = new(ticks: 10);

    protected Humanoid(Model character, PhysicalCharacter physicalCharacter)
    {
        Character = character;
        PhysicalCharacter = physicalCharacter;
    }

    public void Render(Shader shader, float scale, Vector3 cameraPosition)
        => Character.Render(PhysicalCharacter.Pose, shader, scale, cameraPosition);

    public virtual void ContactCallback(ContactInfo collisionInfo, Dictionary<BodyHandle, ISimulationMember> simulationMembers)
    {
        var pair = collisionInfo.CollidablePair;
        var collidableReference
            = pair.A.BodyHandle == BodyHandle ? pair.B : pair.A;
        if (collidableReference.Mobility != CollidableMobility.Dynamic)
            return;

        if (collidableReference.BodyHandle == LastContactBody
            && LastContactTime - DateTime.Now < EpsTime)
            return;

        LastContactTime = DateTime.Now;
        LastContactBody = collidableReference.BodyHandle;
#if DEBUG
        // TODO replace with something more sensible
        if (simulationMembers.TryGetValue(collidableReference.BodyHandle, out var otherBody))
        {
            Console.WriteLine($"Bot collided with {otherBody}");
        }
        else
        {
            Console.WriteLine("Bot collided with something");
        }
#endif
    }

    public virtual void UpdateCharacterGoals(Simulation simulation, float time)
    {
        
    }

    public void Dispose()
    {
        PhysicalCharacter.Dispose();
    }
}
