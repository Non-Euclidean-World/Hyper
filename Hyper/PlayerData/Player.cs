using BepuPhysics;
using BepuUtilities;
using Hyper.Animation.Characters.Cowboy;
using Hyper.TypingUtils;
using Hyper.UserInput;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Hyper.PlayerData;

internal class Player : IInputSubscriber
{
    public Cowboy Character { get; init; }

    public Player()
    {
        Character = new Cowboy(scale: 0.04f);

        RegisterCallbacks();
    }

    public void Update(RigidPose bodyPose, Camera camera)
    {
        var front = camera.Front;
        front.Y = 0;
        float angle = MathF.Atan2(front.X, front.Z);
        RigidPose playerPose = bodyPose;
        playerPose.Orientation = QuaternionEx.CreateFromAxisAngle(new System.Numerics.Vector3(0, 1, 0), angle);
        Character.RigidPose = playerPose;
        camera.ReferencePointPosition = Conversions.ToOpenTKVector(bodyPose.Position)
            + (camera.FirstPerson ? Vector3.Zero : GetThirdPersonOffset(camera));
    }

    public void Render(Shader shader, float scale, Vector3 cameraPosition, bool isFirstPerson)
    {
        if (!isFirstPerson)
            Character.RenderFullDescription(shader, scale, cameraPosition, Cowboy.LocalTranslation);
    }

    private static Vector3 GetThirdPersonOffset(Camera camera)
        => camera.Up * 1f - camera.Front * 5f;

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