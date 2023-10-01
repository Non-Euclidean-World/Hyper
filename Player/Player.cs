﻿using BepuPhysics;
using BepuPhysics.Collidables;
using Character.GameEntities;
using Character.Shaders;
using Common;
using Common.Meshes;
using Common.UserInput;
using OpenTK.Mathematics;
using Physics.Collisions;
using Physics.ContactCallbacks;
using Physics.RayCasting;
using Physics.TypingUtils;
using Player.InventorySystem;

namespace Player;

public class Player : Humanoid, IRayCaster, IContactEventListener
{
    public Inventory Inventory;

    private readonly RayEndpointMarker _rayEndpointMarker;

    private readonly float _rayOffset = 3f; // arbitrary offset to make sure that the ray won't intersect with the player's own collidable

    public float RayMaximumT => 20.0f;

    public System.Numerics.Vector3 RayDirection => Conversions.ToNumericsVector(ViewDirection);

    public System.Numerics.Vector3 RayOrigin => PhysicalCharacter.Pose.Position
            + Conversions.ToNumericsVector(ViewDirection) * _rayOffset;

    public int RayId => 0;

    public Player(PhysicalCharacter physicalCharacter, Context context, int currentSphereId = 0) : base(physicalCharacter, currentSphereId)
    {
        Inventory = new Inventory(context);
        _rayEndpointMarker = new RayEndpointMarker(CubeMesh.Vertices, Vector3.Zero, new Vector3(.5f, .5f, .5f));
    }

    public void Render(ModelShader modelShader, float scale, float curve, Vector3 cameraPosition, bool isFirstPerson)
    {
        if (!isFirstPerson)
            Character.Render(PhysicalCharacter.Pose, modelShader, scale, curve, cameraPosition);
    }

    public void RenderRay(in RayHit rayHit, Shader rayMarkerShader, float scale, float curve, Vector3 cameraPosition)
    {
        _rayEndpointMarker.Position = GetRayEndpoint(rayHit);
        _rayEndpointMarker.Render(rayMarkerShader, scale, curve, cameraPosition);
    }

    // in general this can depend on the properties of the character e.g. size etc
    public Vector3 GetThirdPersonCameraOffset(Camera camera)
        => camera.Up * 1f - camera.Front * 5f;

    public Vector3 GetRayEndpoint(in RayHit hit)
        => Conversions.ToOpenTKVector(RayOrigin) + ViewDirection * hit.T;

    // TODO add new class for Bot to remove hiding
    public new void ContactCallback(ContactInfo collisionInfo, Dictionary<BodyHandle, ISimulationMember> simulationMembers)
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
            Console.WriteLine($"Player collided with {otherBody}");
        }
        else
        {
            Console.WriteLine("Player collided with something");
        }
#endif
    }
}