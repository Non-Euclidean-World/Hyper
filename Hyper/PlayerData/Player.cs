using Assimp.Configs;
using Hyper.Animation.Characters.Cowboy;
using Hyper.UserInput;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Hyper.PlayerData;

internal class Player : IInputSubscriber
{
    public Camera Camera { get; init; }
    
    public Cowboy Character { get; init; }
    
    private bool _isFirstPerson;
    
    public Player(Camera camera)
    {
        Camera = camera;
        Character = new Cowboy(Camera.ReferencePointPosition + GetThirdPersonOffset(), 0.3f);
        _isFirstPerson = true;

        RegisterCallbacks();
    }

    public void Render(Shader shader, float scale, Vector3 cameraPosition)
    {
        // move this part to different method
        Character.Position = Camera.ReferencePointPosition + GetThirdPersonOffset();
        var front = -Camera.Front;
        front.Y = 0;
        Vector3 right = Vector3.Normalize(Vector3.Cross(front, Camera.Up));
        Character.Rotation = new Matrix4(
            new Vector4(right, 0),
            new Vector4(0, 1, 0, 0),
            new Vector4(-front, 0),
            new Vector4(0, 0, 0, 1)
        );
        
        if (!_isFirstPerson) Character.Render(shader, scale, cameraPosition);
    }

    private Vector3 GetThirdPersonOffset() => Camera.Up * -8f + Camera.Front * 8f;
    
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
    }
}