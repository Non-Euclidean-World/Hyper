using BepuPhysics;
using Hyper.Animation.Characters.Cowboy;
using Hyper.Collisions.Bepu;
using Hyper.TypingUtils;
using Hyper.UserInput;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Hyper.GameEntities;

internal class Player : IInputSubscriber
{
    public Cowboy Character { get; init; }

    public PhysicalCharacter PhysicalCharacter { get; init; }


    public Player(PhysicalCharacter physicalCharacter)
    {
        Character = new Cowboy(scale: 0.04f);
        PhysicalCharacter = physicalCharacter;

        RegisterCallbacks();
    }

    public void UpdateCharacterGoals(Simulation simulation, Camera camera, float simulationTimestepDuration, bool tryJump, bool sprint, Vector2 movementDirection)
    {
        Character.RigidPose = PhysicalCharacter.UpdateCharacterGoals(simulation, Conversions.ToNumericsVector(camera.Front), simulationTimestepDuration, tryJump, sprint, Conversions.ToNumericsVector(movementDirection));
    }

    public void Render(Shader shader, float scale, Vector3 cameraPosition, bool isFirstPerson)
    {
        if (!isFirstPerson)
            Character.Render(shader, scale, cameraPosition, Cowboy.LocalTranslation);
    }

    private void UpdateMovementAnimation(Context context)
    {
        if (context.HeldKeys[Keys.W] || context.HeldKeys[Keys.S]
            || context.HeldKeys[Keys.A] || context.HeldKeys[Keys.D])
            Character.Run();
        else
            Character.Idle();
    }

    public void RegisterCallbacks()
    {
        Context context = Context.Instance;
        context.RegisterKeys(new List<Keys>() {
            Keys.F5, Keys.W, Keys.S, Keys.A, Keys.D,
        });

        context.RegisterKeyDownCallback(Keys.W, () => UpdateMovementAnimation(context));
        context.RegisterKeyDownCallback(Keys.S, () => UpdateMovementAnimation(context));
        context.RegisterKeyDownCallback(Keys.A, () => UpdateMovementAnimation(context));
        context.RegisterKeyDownCallback(Keys.D, () => UpdateMovementAnimation(context));

        context.RegisterKeyUpCallback(Keys.W, () => UpdateMovementAnimation(context));
        context.RegisterKeyUpCallback(Keys.S, () => UpdateMovementAnimation(context));
        context.RegisterKeyUpCallback(Keys.A, () => UpdateMovementAnimation(context));
        context.RegisterKeyUpCallback(Keys.D, () => UpdateMovementAnimation(context));
    }
}