using BepuPhysics;
using BepuPhysics.Collidables;
using Character;
using Character.Projectiles;
using Common;
using OpenTK.Mathematics;
using Physics;
using Physics.Collisions;
using Physics.ContactCallbacks;
using Physics.TypingUtils;

namespace Hyper.GameEntities;
public abstract class Humanoid : ISimulationMember, IContactEventListener, IDisposable
{
    public Model Character { get; init; }

    public PhysicalCharacter PhysicalCharacter { get; protected set; }

    public BodyHandle BodyHandle => PhysicalCharacter.BodyHandle;

    public int CurrentSphereId { get; set; }

    public IList<BodyHandle> BodyHandles { get; private set; }

    public bool IsAlive { get; private set; } = true;

    public DateTime DeathTime = DateTime.MinValue;

    protected Vector3 ViewDirection;

    protected DateTime LastContactTime = DateTime.MinValue;

    protected BodyHandle? LastContactBody;

    protected static readonly TimeSpan EpsTime = new(0, 0, 0, 0, milliseconds: 500);

    private int _hp = 3;

    protected Humanoid(Model character, PhysicalCharacter physicalCharacter, int currentSphereId = 0)
    {
        Character = character;
        PhysicalCharacter = physicalCharacter;
        CurrentSphereId = currentSphereId;
        BodyHandles = new BodyHandle[1] { PhysicalCharacter.BodyHandle };
    }

    public void Render(Shader shader, float scale, float curve, Vector3 cameraPosition)
        => Character.Render(PhysicalCharacter.Pose, shader, scale, curve, cameraPosition);

    public virtual void ContactCallback(ContactInfo collisionInfo, SimulationMembers simulationMembers)
    {
        var pair = collisionInfo.CollidablePair;
        var collidableReference
            = pair.A.BodyHandle == BodyHandle ? pair.B : pair.A;
        if (collidableReference.Mobility != CollidableMobility.Dynamic)
            return;

        if (collidableReference.BodyHandle == LastContactBody
            && DateTime.UtcNow - LastContactTime < EpsTime)
            return;

        LastContactTime = DateTime.UtcNow;
        LastContactBody = collidableReference.BodyHandle;

        // TODO replace with something more sensible
        if (simulationMembers.TryGetByHandle(collidableReference.BodyHandle, out var otherBody))
        {
            Console.WriteLine($"Bot collided with {otherBody}");
            if (otherBody is Projectile) // TODO this is terrible we need to change that to IDs in ISimulationMember
            {
                _hp--;
                if (_hp == 0)
                {
                    IsAlive = false;
                    DeathTime = DateTime.UtcNow;
#if DEBUG
                    Console.WriteLine("Bot is dead!");
#endif
                }
            }
        }
        else
        {
#if DEBUG
            Console.WriteLine("Bot collided with something");
#endif
        }

    }

    public virtual void UpdateCharacterGoals(Simulation simulation, float time) { }

    public static PhysicalCharacter CreatePhysicalCharacter(Vector3 position, SimulationManager<PoseIntegratorCallbacks> simulationManager)
        => new(simulationManager.CharacterControllers, simulationManager.Properties, Conversions.ToNumericsVector(position),
            minimumSpeculativeMargin: 0.1f, mass: 1, maximumHorizontalForce: 20, maximumVerticalGlueForce: 100, jumpVelocity: 6, speed: 4,
            maximumSlope: MathF.PI * 0.4f);

    public virtual void Dispose()
    {
        PhysicalCharacter.Dispose();
    }
}
