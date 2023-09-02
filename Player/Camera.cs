using Common.Command;
using Common.UserInput;
using NLog;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Physics.TypingUtils;
using Player.Utils;

namespace Player;

public class Camera : Commandable, IInputSubscriber
{
    public float Curve { get; set; } = 0f;

    public Vector3 ReferencePointPosition { get; set; } = Vector3.Zero;

    public Vector3 Front { get; private set; } = -Vector3.UnitZ;

    public Vector3 Up { get; private set; } = Vector3.UnitY;

    private Vector3 _right = Vector3.UnitX;

    private float _pitch;

    private float _yaw = -MathHelper.PiOver2;

    private float _fov = MathHelper.PiOver2;

    private readonly float _near;

    private readonly float _far;

    private bool _firstMove = true;

    private Vector2 _lastPos;

    private const float Sensitivity = 0.2f;

    public bool FirstPerson { get; set; }

    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public Vector3 ViewPosition { get; private init; }

    public Camera(float aspectRatio, float near, float far, float scale)
    {
        AspectRatio = aspectRatio;
        _near = near;
        _far = far;
        ViewPosition = Vector3.UnitY * scale;

        RegisterCallbacks();
    }

    public float AspectRatio { private get; set; }

    public float Pitch
    {
        get => MathHelper.RadiansToDegrees(_pitch);
        set
        {
            var angle = MathHelper.Clamp(value, -89f, 89f);
            _pitch = MathHelper.DegreesToRadians(angle);
            UpdateVectors();
        }
    }

    public float Yaw
    {
        get => MathHelper.RadiansToDegrees(_yaw);
        set
        {
            _yaw = MathHelper.DegreesToRadians(value);
            UpdateVectors();
        }
    }

    public float Fov
    {
        get => MathHelper.RadiansToDegrees(_fov);
        set
        {
            var angle = MathHelper.Clamp(value, 1f, 90f);
            _fov = MathHelper.DegreesToRadians(angle);
        }
    }

    public Matrix4 GetViewMatrix()
    {
        return Matrices.ViewMatrix(ViewPosition, Front, Up, Curve);
    }

    public Matrix4 GetProjectionMatrix()
    {
        return Matrices.ProjectionMatrix(_fov, _near, _far, AspectRatio, Curve);
    }

    public Matrix4 TranslateMatrix(Vector4 to)
    {
        return Matrices.TranslationMatrix(to, Curve);
    }

    private void UpdateVectors()
    {
        Front = new Vector3(MathF.Cos(_pitch) * MathF.Cos(_yaw), MathF.Sin(_pitch), MathF.Cos(_pitch) * MathF.Sin(_yaw));

        Front = Vector3.Normalize(Front);

        _right = Vector3.Normalize(Vector3.Cross(Front, Vector3.UnitY));
        Up = Vector3.Normalize(Vector3.Cross(_right, Front));
    }

    private void Turn(Vector2 position)
    {
        if (_firstMove)
        {
            _lastPos = position;
            _firstMove = false;
        }
        else
        {
            var deltaX = position.X - _lastPos.X;
            var deltaY = position.Y - _lastPos.Y;
            _lastPos = position;

            Yaw += deltaX * Sensitivity;
            Pitch -= deltaY * Sensitivity; // Reversed since y-coordinates range from bottom to top
        }
    }

    public void UpdateWithCharacter(Player player)
    {
        ReferencePointPosition = Conversions.ToOpenTKVector(player.PhysicalCharacter.Pose.Position)
            + (FirstPerson ? Vector3.Zero : player.GetThirdPersonCameraOffset(this));
    }

    protected override void SetCommand(string[] args)
    {
        switch (args[0])
        {
            case "fov":
                Fov = int.Parse(args[1]);
                break;
            case "curve":
                if (args[1] == "h")
                    Curve = -1f;
                else if (args[1] == "s")
                    Curve = 1f;
                else if (args[1] == "e")
                    Curve = 0f;
                else
                    Curve = float.Parse(args[1]);
                break;
            case "position":
                if (args.Length != 4)
                    return;
                float x = float.Parse(args[1]);
                float y = float.Parse(args[2]);
                float z = float.Parse(args[3]);
                ReferencePointPosition = new Vector3(x, y, z);
                break;
            default:
                CommandNotFound();
                break;
        }
    }

    protected override void GetCommand(string[] args)
    {
        switch (args[0])
        {
            case "fov":
                Console.WriteLine(Fov);
                break;
            case "position":
                Console.WriteLine(ReferencePointPosition);
                break;
            default:
                CommandNotFound();
                break;
        }
    }

    public void RegisterCallbacks()
    {
        Context context = Context.Instance;
        context.RegisterKeys(new List<Keys>() {
            Keys.D8, Keys.D9, Keys.D0, Keys.Down, Keys.Up, Keys.Tab
        });

        context.RegisterKeyDownCallback(Keys.D8, () => Curve = 0f);
        context.RegisterKeyDownCallback(Keys.D9, () => Curve = 1f);
        context.RegisterKeyDownCallback(Keys.D0, () => Curve = -1f);
        context.RegisterKeyHeldCallback(Keys.Down, (e) => Curve -= 1f * (float)e.Time);
        context.RegisterKeyHeldCallback(Keys.Up, (e) => Curve += 1f * (float)e.Time);
        context.RegisterKeyDownCallback(Keys.Tab, () => FirstPerson = !FirstPerson);

        context.RegisterMouseMoveCallback((e) => Turn(e.Position));
    }
}