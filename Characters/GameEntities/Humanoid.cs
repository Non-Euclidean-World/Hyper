using BepuPhysics;
using Character.Characters;
using Character.Characters.Cowboy;
using Common;
using OpenTK.Mathematics;
using Physics.Collisions.Bepu;
using Physics.TypingUtils;

namespace Character.GameEntities;
public class Humanoid
{
    public CowboyModel Character { get; init; }

    public PhysicalCharacter PhysicalCharacter { get; init; }

    protected Vector3 ViewDirection;

    public Humanoid(PhysicalCharacter physicalCharacter)
    {
        Character = new CowboyModel();
        PhysicalCharacter = physicalCharacter;
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

    public void Render(Shader shader, float scale, Vector3 cameraPosition)
        => Character.Render(PhysicalCharacter.Pose, shader, scale, cameraPosition);

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
}
