using Common.UserInput;
using Hyper.PlayerData.Utils;
using OpenTK.Mathematics;

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

    private Vector3 FixedViewPosition => HyperCameraPosition.Multiplier * Vector3.UnitY * _scale;

    public Vector3 ViewPosition =>
        Curve >= 0
            ? ReferencePointPosition * _scale
            : FixedViewPosition;

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

    public void UpdateVectors()
    {
        Front = new Vector3(MathF.Cos(_pitch) * MathF.Cos(_yaw), MathF.Sin(_pitch), MathF.Cos(_pitch) * MathF.Sin(_yaw));


        Front = FrontTransform(Vector3.Normalize(Front));

        _right = Vector3.Normalize(Vector3.Cross(Front, Vector3.UnitY));
        Up = Vector3.Normalize(Vector3.Cross(_right, Front));
    }

    private void Turn(Vector2 delta)
    {
        Yaw += delta.X * Sensitivity;
        Pitch -= delta.Y * Sensitivity; // Reversed since y-coordinates range from bottom to top
    }

    public void RegisterCallbacks(Context context)
    {
        context.RegisterMouseMoveCallback((e) => Turn(e.Delta));
    }
}