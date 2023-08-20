using BepuPhysics;
using Hyper.Animation.Characters.Cowboy;
using Hyper.Collisions.Bepu;
using Hyper.TypingUtils;
using OpenTK.Mathematics;

namespace Hyper.GameEntities;

internal class Player
{
    public CowboyModel Character { get; init; }

    public PhysicalCharacter PhysicalCharacter { get; init; }

    public Player(PhysicalCharacter physicalCharacter)
    {
        Character = new CowboyModel();
        PhysicalCharacter = physicalCharacter;
    }

    public void UpdateCharacterGoals(Simulation simulation, Camera camera, float simulationTimestepDuration, bool tryJump, bool sprint, Vector2 movementDirection)
    {
        if (movementDirection != Vector2.Zero)
        {
            UpdateMovementAnimation(Animation.Characters.CharacterAnimationType.Walk);
        }
        else
        {
            UpdateMovementAnimation(Animation.Characters.CharacterAnimationType.Stand);
        }

        PhysicalCharacter.UpdateCharacterGoals(simulation, Conversions.ToNumericsVector(camera.Front), simulationTimestepDuration, tryJump, sprint, Conversions.ToNumericsVector(movementDirection));
    }

    public void Render(Shader shader, float scale, Vector3 cameraPosition, bool isFirstPerson)
    {
        if (!isFirstPerson)
            Character.Render(PhysicalCharacter.Pose, shader, scale, cameraPosition);
    }

    // in general this can depend on the properties of the character e.g. size etc
    public Vector3 GetThirdPersonCameraOffset(Camera camera)
        => camera.Up * 1f - camera.Front * 5f;

    public System.Numerics.Vector3 GetCharacterRay(Vector3 viewDirection, float length)
        => PhysicalCharacter.Pose.Position + Conversions.ToNumericsVector(viewDirection) * length;

    private void UpdateMovementAnimation(Animation.Characters.CharacterAnimationType animationType)
    {
        switch (animationType)
        {
            case Animation.Characters.CharacterAnimationType.Walk:
            case Animation.Characters.CharacterAnimationType.Run: // TODO we need different animations for walking and running
                Character.Run(); break;
            case Animation.Characters.CharacterAnimationType.Stand:
                Character.Idle(); break;
            case Animation.Characters.CharacterAnimationType.Jump:
                throw new NotImplementedException();
            default:
                Character.Idle(); break;
        }
    }
}