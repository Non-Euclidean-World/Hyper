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
public class Humanoid : ISimulationMember, IContactEventListener
{
    public CowboyModel Character { get; init; }

    public PhysicalCharacter PhysicalCharacter { get; init; }

    public BodyHandle BodyHandle => PhysicalCharacter.BodyHandle;

    public int CurrentSphereId { get; set; }

    protected Vector3 ViewDirection;

    protected DateTime LastContactTime = DateTime.MinValue;

    protected BodyHandle? LastContactBody;

    protected static readonly TimeSpan EpsTime = new(0, 0, 0, 0, milliseconds: 500);

    public Humanoid(PhysicalCharacter physicalCharacter, int currentSphereId = 0)
    {
        Character = new CowboyModel();
        PhysicalCharacter = physicalCharacter;
        CurrentSphereId = currentSphereId;
    }

    public void UpdateCharacterGoals(Simulation simulation, Vector3 viewDirection, float simulationTimestepDuration, bool tryJump, bool sprint, Vector2 movementDirection)
    {
        if (movementDirection != Vector2.Zero)
        {
            UpdateMovementAnimation(CharacterAnimationType.Walk);
        }
        else
        {
            UpdateMovementAnimation(CharacterAnimationType.Stand);
        }

        PhysicalCharacter.UpdateCharacterGoals(simulation, Conversions.ToNumericsVector(viewDirection), simulationTimestepDuration, tryJump, sprint, Conversions.ToNumericsVector(movementDirection));
        ViewDirection = viewDirection;
    }

    public void Render(Shader shader, float scale, float curve, Vector3 cameraPosition)
        => Character.Render(PhysicalCharacter.Pose, shader, scale, curve, cameraPosition);

    private void UpdateMovementAnimation(CharacterAnimationType animationType)
    {
        switch (animationType)
        {
            case CharacterAnimationType.Walk:
            case CharacterAnimationType.Run: // TODO we need different animations for walking and running
                Character.Run(); break;
            case CharacterAnimationType.Stand:
                Character.Idle(); break;
            case CharacterAnimationType.Jump:
                throw new NotImplementedException();
            default:
                Character.Idle(); break;
        }
    }

    public void ContactCallback(ContactInfo collisionInfo, Dictionary<BodyHandle, ISimulationMember> simulationMembers)
    {
        var pair = collisionInfo.CollidablePair;
        var collidableReference
            = pair.A.BodyHandle == BodyHandle ? pair.B : pair.A;
        if (collidableReference.Mobility != CollidableMobility.Dynamic)
            return;

        if (collidableReference.BodyHandle == LastContactBody
            && DateTime.Now - LastContactTime < EpsTime)
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

    public void Dispose()
    {
        PhysicalCharacter.Dispose();
        // TODO Models should be singletons so we don't need to dispose them. however now they are not so we have to keep that in mind.
    }
}
