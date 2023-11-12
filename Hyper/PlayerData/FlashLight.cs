using OpenTK.Mathematics;

namespace Hyper.PlayerData;
internal class FlashLight
{
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

    public FlashLight(Vector3 color, float cutOff, float outerCutOff, Vector3 ambient, Vector3 diffuse, Vector3 specular, float constant, float linear, float quadratic)
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
    }

    public FlashLight()
    {
        Color = new Vector3(1, 1, 1);
        CutOff = MathF.Cos(MathHelper.DegreesToRadians(15f));
        OuterCutOff = MathF.Cos(MathHelper.DegreesToRadians(30f));
        Ambient = new Vector3(0.1f, 0.1f, 0.1f);
        Diffuse = new Vector3(0.8f, 0.8f, 0.8f);
        Specular = new Vector3(1f, 1f, 1f);
        Constant = 1f;
        Linear = 0.09f;
        Quadratic = 0.032f;
    }

    public static FlashLight CreatePlayerFlashlight()
    {
        return new FlashLight(color: new Vector3(1, 1, 1),
            cutOff: MathF.Cos(MathHelper.DegreesToRadians(15f)),
            outerCutOff: MathF.Cos(MathHelper.DegreesToRadians(30f)),
            ambient: new Vector3(0.1f, 0.1f, 0.1f),
            diffuse: new Vector3(0.8f, 0.8f, 0.8f),
            specular: new Vector3(1f, 1f, 1f),
            constant: 1f,
            linear: 0.09f,
            quadratic: 0.032f);
    }
}
