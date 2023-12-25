using BepuPhysics;
using BepuPhysics.Collidables;
using Character;
using Character.Characters;
using Character.Projectiles;
using Common;
using Common.Meshes;
using Common.UserInput;
using Hyper.GameEntities;
using Hyper.PlayerData.InventorySystem;
using OpenTK.Mathematics;
using Physics;
using Physics.Collisions;
using Physics.ContactCallbacks;
using Physics.RayCasting;
using Physics.TypingUtils;

namespace Hyper.PlayerData;

internal class Player : Humanoid, IRayCaster
{
    public readonly Inventory Inventory;

    public readonly FlashLight FlashLight;

    private readonly RayEndpointMarker _rayEndpointMarker;

    private const float RayOffset = 3f; // arbitrary offset to make sure that the ray won't intersect with the player's own collidable

    public float RayMaximumT => 20.0f;

    public readonly int MaxHP = 20;

    public int HP { get; private set; } = 20;

    public System.Numerics.Vector3 RayDirection
    {
        get =>
            Conversions.ToNumericsVector(ViewDirection);
    }

    public System.Numerics.Vector3 RayOrigin => PhysicalCharacter.Pose.Position
            + RayDirection * RayOffset;

    public int RayId => 0;

    public bool Hidden { get; private set; }

    public Player(PhysicalCharacter physicalCharacter, Context context, int currentSphereId = 0) : base(
        new Model(AstronautResources.Instance, localScale: 0.45f, localTranslation: new Vector3(0, -4.4f, 0)), physicalCharacter, currentSphereId)
    {
        Inventory = new Inventory(context, starterItems: true);
        FlashLight = new FlashLight(CurrentSphereId);
        _rayEndpointMarker = new RayEndpointMarker(new Vector3(.5f, .5f, .5f));
    }

    public void Render(Shader modelShader, float scale, float curve, Vector3 cameraPosition, bool isFirstPerson)
    {
        if (!isFirstPerson && !Hidden)
            Character.Render(PhysicalCharacter.Pose, modelShader, scale, curve, cameraPosition);
    }

    public void RenderRay(in RayHit rayHit, Shader rayMarkerShader, float scale, float curve, Vector3 cameraPosition, float size)
    {
        var rayPosition = GetRayEndpoint(rayHit);
        _rayEndpointMarker.Render(rayMarkerShader, scale, curve, rayPosition, cameraPosition, size);
    }

    public Vector3 GetRayEndpoint(in RayHit hit)
        => Conversions.ToOpenTKVector(RayOrigin) + Conversions.ToOpenTKVector(RayDirection) * hit.T;

    public override void ContactCallback(ContactInfo collisionInfo, SimulationMembers simulationMembers)
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
#if DEBUG
        // TODO replace with something more sensible
        if (simulationMembers.TryGetByHandle(collidableReference.BodyHandle, out var otherBody))
        {
            Console.WriteLine($"Player collided with {otherBody}");
            if (otherBody.GetType() == typeof(Projectile))
            {
                HP--;
            }
        }
        else
        {
            Console.WriteLine("Player collided with something");
        }
#endif
    }

    public void UpdateCharacterGoals(Simulation simulation, Vector3 viewDirection, float time, bool tryJump, bool sprint, Vector2 movementDirection)
    {
        if (movementDirection != Vector2.Zero)
        {
            if (sprint)
                Character.Animator.Play(1);
            else
                Character.Animator.Play(0);
        }
        else
        {
            Character.Animator.Reset();
        }

        PhysicalCharacter.UpdateCharacterGoals(simulation, Conversions.ToNumericsVector(viewDirection), time, tryJump, sprint, Conversions.ToNumericsVector(movementDirection));
        ViewDirection = viewDirection;
        FlashLight.Position =
             (CurrentSphereId == 0 ? Vector3.Cross(ViewDirection, Vector3.UnitY) : -Vector3.Cross(ViewDirection, Vector3.UnitY))
             + Conversions.ToOpenTKVector(PhysicalCharacter.Pose.Position); // right hand
        FlashLight.Direction = ViewDirection;
    }

    public void Hide()
    {
        if (Hidden)
            return;

        PhysicalCharacter.Dispose();
        Hidden = true;
    }

    public void Show(PhysicalCharacter physicalCharacter)
    {
        if (!Hidden)
            return;

        PhysicalCharacter = physicalCharacter;
        BodyHandles = new BodyHandle[1] { physicalCharacter.BodyHandle };
        Hidden = false;
    }

    public override void Dispose()
    {
        if (!Hidden)
            base.Dispose();
    }
}