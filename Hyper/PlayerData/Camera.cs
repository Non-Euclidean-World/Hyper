using Common.UserInput;
using Hyper.PlayerData.Utils;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Hyper.PlayerData;

internal class Camera : IInputSubscriber
{
    public float Curve { get; private init; }

    public Vector3 ReferencePointPosition { get; set; } = Vector3.Zero;

    public Vector3 Front { get; private set; } = -Vector3.UnitZ;

    public Vector3 Up { get; private set; } = Vector3.UnitY;

    private Vector3 _right = Vector3.UnitX;

    private float _pitch;

    private float _yaw = -MathHelper.PiOver2;

    private float _fov = MathHelper.PiOver2;

    private readonly float _near;

    private readonly float _far;

    private readonly float _scale;

    private readonly Vector3 _fixedViewPosition;

    public Vector3 ViewPosition
    {
        get => Curve >= 0
            ? ReferencePointPosition * _scale
            : _fixedViewPosition;
    }

    /// <summary>
    /// Sphere the camera is in
    /// </summary>
    public int Sphere { get; set; } = 0;

    /// <summary>
    /// Center of the sphere the camera is in
    /// </summary>
    public Vector3 SphereCenter { get; set; }

    private const float Sensitivity = 0.2f;

    public bool FirstPerson { get; set; }

    /// <summary>
    /// Transformation applied to the camera front vector
    /// </summary>
    public Func<Vector3, Vector3> FrontTransform { get; set; } = IdentityTransform;

    public static readonly Func<Vector3, Vector3> IdentityTransform = (Vector3 v) => v;

    public Camera(float aspectRatio, float curve, float near, float far, float scale, Context context)
    {
        AspectRatio = aspectRatio;
        Curve = curve;
        _near = near;
        _far = far;
        _fixedViewPosition = HyperCameraPosition.Multiplier * Vector3.UnitY * scale;
        _scale = scale;

        RegisterCallbacks(context);
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
        return Matrices.ViewMatrix(ViewPosition, Front, Up, Curve, Sphere, SphereCenter * _scale);
    }

    public Matrix4 GetProjectionMatrix()
    {
        return Matrices.ProjectionMatrix(_fov, _near, _far, AspectRatio, Curve);
    }

    public Matrix4 GetTranslationMatrix(Vector4 to)
    {
        return Matrices.TranslationMatrix(to, Curve);
    }

    public void UpdateVectors()
    {
        float adjustedPitch = _pitch;
        float adjustedYaw = _yaw;
        Front = new Vector3(MathF.Cos(adjustedPitch) * MathF.Cos(adjustedYaw), MathF.Sin(adjustedPitch), MathF.Cos(adjustedPitch) * MathF.Sin(adjustedYaw));


        Front = FrontTransform(Vector3.Normalize(Front));

        _right = Vector3.Normalize(Vector3.Cross(Front, Sphere == 1 ? -Vector3.UnitY : Vector3.UnitY));
        Up = Vector3.Normalize(Vector3.Cross(_right, Front));
    }

    private void Turn(Vector2 delta)
    {
        Yaw += delta.X * Sensitivity;
        Pitch -= delta.Y * Sensitivity; // Reversed since y-coordinates range from bottom to top
    }

    public void RegisterCallbacks(Context context)
    {
        context.RegisterKeys(new List<Keys>() {
            Keys.Tab
        });

        context.RegisterKeyDownCallback(Keys.Tab, () => FirstPerson = !FirstPerson);

        context.RegisterMouseMoveCallback((e) => Turn(e.Delta));
    }
}