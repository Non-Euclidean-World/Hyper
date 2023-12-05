using OpenTK.Mathematics;

namespace Common.Meshes;

/// <summary>
/// Class representing a spot light
/// </summary>
public class FlashLight
{
    public bool Active;

    public Vector3 Position { get; set; }
    public Vector3 Color { get; set; }
    public Vector3 Direction { get; set; }
    public float CutOff { get; private init; }
    public float OuterCutOff { get; private init; }
    public Vector3 Ambient { get; private init; }
    public Vector3 Diffuse { get; private init; }
    public Vector3 Specular { get; private init; }
    public float Constant { get; private init; }
    public float Linear { get; private init; }
    public float Quadratic { get; private init; }
    public int CurrentSphereId { get; set; }

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
    /// Initializes a new instance of the <c>FlashLight</c> class with default values
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
    /// Creates a light source for car's headlights
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
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
    /// Creates a light source for car's tail lights
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
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
