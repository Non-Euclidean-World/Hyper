using BepuPhysics;
using Common;
using Hyper.Animation.Characters.Cowboy;
using OpenTK.Mathematics;
using Physics.Collisions.Bepu;
using Physics.TypingUtils;

namespace Hyper.GameEntities;
internal class Humanoid
{
    public CowboyModel Character { get; init; }

    public PhysicalCharacter PhysicalCharacter { get; init; }

    public Humanoid(PhysicalCharacter physicalCharacter)
    {
        Character = new CowboyModel();
        PhysicalCharacter = physicalCharacter;
    }

    public void UpdateCharacterGoals(Simulation simulation, Vector3 viewDirection, float simulationTimestepDuration, bool tryJump, bool sprint, Vector2 movementDirection)
    {
        if (movementDirection != Vector2.Zero)
        {
            UpdateMovementAnimation(Animation.Characters.CharacterAnimationType.Walk);
        }
        else
        {
            UpdateMovementAnimation(Animation.Characters.CharacterAnimationType.Stand);
        }

        PhysicalCharacter.UpdateCharacterGoals(simulation, Conversions.ToNumericsVector(viewDirection), simulationTimestepDuration, tryJump, sprint, Conversions.ToNumericsVector(movementDirection));
    }

    public void Render(Shader shader, float scale, Vector3 cameraPosition)
        => Character.Render(PhysicalCharacter.Pose, shader, scale, cameraPosition);

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
