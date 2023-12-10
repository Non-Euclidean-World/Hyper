using OpenTK.Mathematics;

namespace Common.Meshes;

/// <summary>
/// Class representing a spot light
/// </summary>
public class FlashLight
{
    /// <summary>
    /// Indicates whether the flashlight is active or not.
    /// </summary>
    public bool Active;

    /// <summary>
    /// The position of the flashlight in 3D space.
    /// </summary>
    public Vector3 Position { get; set; }

    /// <summary>
    /// The color of the flashlight.
    /// </summary>
    public Vector3 Color { get; set; }

    /// <summary>
    /// The direction the flashlight is pointing.
    /// </summary>
    public Vector3 Direction { get; set; }

    /// <summary>
    /// The inner angle of the spotlight cone.
    /// </summary>
    public float CutOff { get; private init; }

    /// <summary>
    /// The outer angle of the spotlight cone.
    /// </summary>
    public float OuterCutOff { get; private init; }

    /// <summary>
    /// The ambient light color emitted by the flashlight.
    /// </summary>
    public Vector3 Ambient { get; private init; }

    /// <summary>
    /// The diffuse light color emitted by the flashlight.
    /// </summary>
    public Vector3 Diffuse { get; private init; }

    /// <summary>
    /// The specular light color emitted by the flashlight.
    /// </summary>
    public Vector3 Specular { get; private init; }

    /// <summary>
    /// The constant attenuation factor of the flashlight.
    /// </summary>
    public float Constant { get; private init; }

    /// <summary>
    /// The linear attenuation factor of the flashlight.
    /// </summary>
    public float Linear { get; private init; }

    /// <summary>
    /// The quadratic attenuation factor of the flashlight.
    /// </summary>
    public float Quadratic { get; private init; }

    /// <summary>
    /// The ID of the hypersphere the flashlight is located in.
    /// </summary>
    public int CurrentSphereId { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FlashLight"/> class with specified parameters.
    /// </summary>
    public FlashLight(Vector3 color, float cutOff, float outerCutOff, Vector3 ambient, Vector3 diffuse, Vector3 specular, float constant, float linear, float quadratic, int currentSphereId)
    {
        Color = color;
        CutOff = cutOff;
        OuterCutOff = outerCutOff;
        Ambient = ambient;
        Diffuse = diffuse;
        Specular = specular;
        Constant = constant;
        Linear = linear;
        Quadratic = quadratic;
        CurrentSphereId = currentSphereId;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FlashLight"/> class with default values
    /// </summary>
    public FlashLight(int sphere)
    {
        Color = new Vector3(1, 1, 1);
        CutOff = MathF.Cos(MathHelper.DegreesToRadians(15f));
        OuterCutOff = MathF.Cos(MathHelper.DegreesToRadians(30f));
        Ambient = new Vector3(0.3f, 0.3f, 0.3f);
        Diffuse = new Vector3(0.95f, 0.95f, 0.95f);
        Specular = new Vector3(1f, 1f, 1f);
        Constant = 1f;
        Linear = 0.2f;
        Quadratic = 0.3f;
        CurrentSphereId = sphere;
    }

    /// <summary>
    /// Creates a spotlight for car's headlights with specified color.
    /// </summary>
    /// <param name="color">The color of the headlight.</param>
    /// <param name="sphere">The ID of the sphere the headlight is in..</param>
    /// <returns>A spotlight representing a car's headlight.</returns>
    public static FlashLight CreateCarLight(Vector3 color, int sphere)
    {
        return new FlashLight(color,
            cutOff: MathF.Cos(MathHelper.DegreesToRadians(15f)),
            outerCutOff: MathF.Cos(MathHelper.DegreesToRadians(30f)),
            ambient: new Vector3(0.1f, 0.1f, 0.1f),
            diffuse: new Vector3(0.8f, 0.8f, 0.8f),
            specular: new Vector3(1f, 1f, 1f),
            constant: 1f,
            linear: 0.1f,
            quadratic: 0.04f,
            sphere);
    }

    /// <summary>
    /// Creates a spotlight for car's tail lights with specified color.
    /// </summary>
    /// <param name="color">The color of the tail light.</param>
    /// <param name="sphere">The ID of the sphere associated with the tail light.</param>
    /// <returns>A spotlight representing a car's tail light.</returns>
    public static FlashLight CreateCarRearLight(Vector3 color, int sphere)
    {
        return new FlashLight(color,
            cutOff: MathF.Cos(MathHelper.DegreesToRadians(15f)),
            outerCutOff: MathF.Cos(MathHelper.DegreesToRadians(30f)),
            ambient: new Vector3(0.1f, 0.1f, 0.1f),
            diffuse: new Vector3(0.6f, 0.6f, 0.6f),
            specular: new Vector3(1f, 1f, 1f),
            constant: 1f,
            linear: 0.5f,
            quadratic: 0.8f,
            sphere);
    }
}
