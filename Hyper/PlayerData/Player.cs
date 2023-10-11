﻿using BepuPhysics;
using BepuPhysics.Collidables;
using Character;
using Character.Characters;
using Character.GameEntities;
using Common;
using Common.Meshes;
using Common.UserInput;
using Hyper.PlayerData.InventorySystem;
using OpenTK.Mathematics;
using Physics.Collisions;
using Physics.ContactCallbacks;
using Physics.RayCasting;
using Physics.TypingUtils;

namespace Hyper.PlayerData;

internal class Player : Humanoid, IRayCaster
{
    public readonly Inventory Inventory;

    private readonly RayEndpointMarker _rayEndpointMarker;

    private const float RayOffset = 3f; // arbitrary offset to make sure that the ray won't intersect with the player's own collidable

    public float RayMaximumT => 20.0f;

    public System.Numerics.Vector3 RayDirection => Conversions.ToNumericsVector(ViewDirection);

    public System.Numerics.Vector3 RayOrigin => PhysicalCharacter.Pose.Position
            + Conversions.ToNumericsVector(ViewDirection) * RayOffset;

    public int RayId => 0;

    public Player(PhysicalCharacter physicalCharacter, Context context, int currentSphereId = 0) : base(
        new Model(CowboyResources.Instance, localScale: 0.4f, localTranslation: new Vector3(0, -5, 0)), physicalCharacter, currentSphereId)
    {
        Inventory = new Inventory(context, starterItems: true);
        _rayEndpointMarker = new RayEndpointMarker(CubeMesh.Vertices, Vector3.Zero, new Vector3(.5f, .5f, .5f));
    }

    public void Render(Shader modelShader, float scale, float curve, Vector3 cameraPosition, bool isFirstPerson)
    {
        if (!isFirstPerson)
            Character.Render(PhysicalCharacter.Pose, modelShader, scale, curve, cameraPosition);
    }

    public void RenderRay(in RayHit rayHit, Shader rayMarkerShader, float scale, float curve, Vector3 cameraPosition)
    {
        _rayEndpointMarker.Position = GetRayEndpoint(rayHit);
        _rayEndpointMarker.Render(rayMarkerShader, scale, curve, cameraPosition);
    }

    public Vector3 GetRayEndpoint(in RayHit hit)
        => Conversions.ToOpenTKVector(RayOrigin) + ViewDirection * hit.T;

    public override void ContactCallback(ContactInfo collisionInfo, Dictionary<BodyHandle, ISimulationMember> simulationMembers)
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
            Console.WriteLine($"Player collided with {otherBody}");
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
            Character.Animator.Play(0);
        }
        else
        {
            Character.Animator.Reset();
        }

        PhysicalCharacter.UpdateCharacterGoals(simulation, Conversions.ToNumericsVector(viewDirection), time, tryJump, sprint, Conversions.ToNumericsVector(movementDirection));
        ViewDirection = viewDirection;
    }
}