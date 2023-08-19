using BepuPhysics;
using Hyper.Animation.Characters.Cowboy;
using Hyper.Collisions.Bepu;
using Hyper.TypingUtils;
using OpenTK.Mathematics;

namespace Hyper.GameEntities;
internal class Bot
{
    public Cowboy Character { get; init; }

    public PhysicalCharacter PhysicalCharacter { get; init; }

    public Bot(PhysicalCharacter physicalCharacter)
    {
        Character = new Cowboy(scale: 0.04f);
        PhysicalCharacter = physicalCharacter;
    }

    public void UpdateCharacterGoals(Simulation simulation, Vector3 viewDirection, float simulationTimestepDuration, bool tryJump, bool sprint, Vector2 movementDirection)
    {
        Character.RigidPose = PhysicalCharacter.UpdateCharacterGoals(simulation, Conversions.ToNumericsVector(viewDirection), simulationTimestepDuration, tryJump, sprint, Conversions.ToNumericsVector(movementDirection));
    }

    public void Render(Shader shader, float scale, Vector3 cameraPosition)
     => Character.Render(shader, scale, cameraPosition, Cowboy.LocalTranslation);
}
