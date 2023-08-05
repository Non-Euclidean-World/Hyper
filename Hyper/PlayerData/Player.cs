using Hyper.Animation.Characters.Cowboy;
using Hyper.UserInput;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Hyper.PlayerData;

internal class Player : IInputSubscriber
{
    public Camera Camera { get; init; }
    
    private Cowboy Character { get; init; }
    
    private bool _isFirstPerson;
    
    public Player(Camera camera)
    {
        Camera = camera;
        Character = new Cowboy(Camera.ReferencePointPosition + GetThirdPersonOffset(), 0.3f);
        _isFirstPerson = true;

        RegisterCallbacks();
    }

    private void Update()
    {
        Character.Position = Camera.ReferencePointPosition + GetThirdPersonOffset();
        var front = Camera.Front;
        front.Y = 0;
        float angle = (float)Math.Atan2(front.X, front.Z);
        Character.Rotation = Matrix4.CreateRotationY(angle);
    }
    
    public void Render(Shader shader, float scale, Vector3 cameraPosition)
    {
        if (!_isFirstPerson) Character.Render(shader, scale, cameraPosition);
    }

    private Vector3 GetThirdPersonOffset() => Camera.Up * -8f + Camera.Front * 12f;
    
    private void SetCameraPerson()
    {
        if (_isFirstPerson)
        {
            _isFirstPerson = false;
            Camera.ReferencePointPosition -= GetThirdPersonOffset();

            return;
        }
        
        _isFirstPerson = true;
        Camera.ReferencePointPosition += GetThirdPersonOffset();
    }

    private void UpdateMovementAnimation(Context context)
    {
        if (context.HeldKeys[Keys.W] || context.HeldKeys[Keys.S] || context.HeldKeys[Keys.A] ||
            context.HeldKeys[Keys.D])
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
        
        context.RegisterKeyDownCallback(Keys.F5, SetCameraPerson);
        
        context.RegisterKeyDownCallback(Keys.W, () => UpdateMovementAnimation(context));
        context.RegisterKeyDownCallback(Keys.S, () => UpdateMovementAnimation(context));
        context.RegisterKeyDownCallback(Keys.A, () => UpdateMovementAnimation(context));
        context.RegisterKeyDownCallback(Keys.D, () => UpdateMovementAnimation(context));
        
        context.RegisterKeyUpCallback(Keys.W, () => UpdateMovementAnimation(context));
        context.RegisterKeyUpCallback(Keys.S, () => UpdateMovementAnimation(context));
        context.RegisterKeyUpCallback(Keys.A, () => UpdateMovementAnimation(context));
        context.RegisterKeyUpCallback(Keys.D, () => UpdateMovementAnimation(context));
        
        context.RegisterUpdateFrameCallback((_) => Update());
    }
}