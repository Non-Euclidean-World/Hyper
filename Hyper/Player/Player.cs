using Hyper.Animation.Characters.Cowboy;
using Hyper.UserInput;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Hyper.Player;

internal class Player : IInputSubscriber
{
    public Camera Camera { get; init; }
    
    public Cowboy Character { get; set; }
    
    private bool _isFirstPerson;
    
    public Player(float aspectRatio, float near, float far, float scale)
    {
        Camera = new Camera(aspectRatio, near, far, scale);
        Character = new Cowboy();
        _isFirstPerson = true;

        RegisterCallbacks();
    }

    public void Render(Shader shader, float scale, Vector3 cameraPosition)
    {
        if (!_isFirstPerson) Character.Render(shader, scale, cameraPosition);
    }

    private void SetCameraPerson()
    {
        if (_isFirstPerson)
        {
            _isFirstPerson = false;
            Camera.ReferencePointPosition += Camera.Up * 0.5f + Camera.Front * -0.5f;
        }
        
        _isFirstPerson = true;
        Camera.ReferencePointPosition += Camera.Up * -0.5f + Camera.Front * 0.5f;
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