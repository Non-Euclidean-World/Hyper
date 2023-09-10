using Character.GameEntities;
using Character.Shaders;
using Common;
using Common.Meshes;
using OpenTK.Mathematics;
using Physics.Collisions.Bepu;
using Physics.RayCasting;
using Physics.TypingUtils;

namespace Player;

public class Player : Humanoid, IRayCaster
{
    private readonly RayEndpointMarker _rayEndpointMarker;

    private readonly float _rayOffset = 3f; // arbitrary offset to make sure that the ray won't intersect with the player's own collidable

    public float RayMaximumT => 20.0f;

    public System.Numerics.Vector3 RayDirection => Conversions.ToNumericsVector(ViewDirection);

    public System.Numerics.Vector3 RayOrigin => PhysicalCharacter.Pose.Position
            + Conversions.ToNumericsVector(ViewDirection) * _rayOffset;

    public int RayId => 0;

    public Player(PhysicalCharacter physicalCharacter) : base(physicalCharacter)
    {
        _rayEndpointMarker = new RayEndpointMarker(CubeMesh.Vertices, Vector3.Zero, new Vector3(.5f, .5f, .5f));
    }

    public void Render(ModelShader modelShader, float scale, Vector3 cameraPosition, bool isFirstPerson)
    {
        if (!isFirstPerson)
            Character.Render(PhysicalCharacter.Pose, modelShader, scale, cameraPosition);
    }

    public void RenderRay(in RayHit rayHit, Shader rayMarkerShader, float scale, Vector3 cameraPosition)
    {
        _rayEndpointMarker.Position = GetRayEndpoint(rayHit);
        _rayEndpointMarker.Render(rayMarkerShader, scale, cameraPosition);
    }

    // in general this can depend on the properties of the character e.g. size etc
    public Vector3 GetThirdPersonCameraOffset(Camera camera)
        => camera.Up * 1f - camera.Front * 5f;

    public Vector3 GetRayEndpoint(in RayHit hit)
        => Conversions.ToOpenTKVector(RayOrigin) + ViewDirection * hit.T;
}