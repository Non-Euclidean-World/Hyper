using BepuPhysics;
using Common;
using Common.Meshes;
using Common.ResourceClasses;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Physics.Collisions;

namespace Character.LightSources;

/// <summary>
/// Class representing a point light source.
/// </summary>
public class Lamp : ISimulationMember
{
    /// <summary>
    /// Color of the lamp.
    /// </summary>
    public Vector3 Color { get; set; }

    /// <summary>
    /// Ambient color of the lamp.
    /// </summary>
    public Vector3 Ambient { get; private init; }

    /// <summary>
    /// Diffuse color of the lamp.
    /// </summary>
    public Vector3 Diffuse { get; private init; }

    /// <summary>
    /// Specular color of the lamp.
    /// </summary>
    public Vector3 Specular { get; private init; }

    /// <summary>
    /// The constant attenuation factor.
    /// </summary>
    public float Constant { get; private init; }

    /// <summary>
    /// The linear attenuation factor.
    /// </summary>
    public float Linear { get; private init; }

    /// <summary>
    /// The quadratic attenuation factor.
    /// </summary>
    public float Quadratic { get; private init; }

    /// <summary>
    /// List of body handles of the lamp.
    /// </summary>
    /// <exception cref="NotImplementedException">This should not be used. It's only because <see cref="Lamp"/> implements <see cref="ISimulationMember"/>.</exception>
    public IList<BodyHandle> BodyHandles => throw new NotImplementedException();

    /// <summary>
    /// The id of the sphere the lamp is in.
    /// </summary>
    public int CurrentSphereId { get; set; }

    public Vector3 Position;
    
    private readonly SphereResource _sphereResource = SphereResource.Instance;
    
    private Lamp(Vector3 position, Vector3 color, Vector3 ambient, Vector3 diffuse, Vector3 specular, float constant, float linear, float quadratic, int sphereId)
    {
        Position = position;
        Color = color;
        Ambient = ambient;
        Diffuse = diffuse;
        Specular = specular;
        Constant = constant;
        Linear = linear;
        Quadratic = quadratic;
        CurrentSphereId = sphereId;
    }

    /// <summary>
    /// Creates a lamp (a glowing cube).
    /// </summary>
    /// <param name="position">The position of the lamp.</param>
    /// <param name="color">The color of the lamp.</param>
    /// <param name="sphereId">The id of the sphere the lamp is in.</param>
    /// <returns>An instance of the <see cref="Lamp"/> class.</returns>
    public static Lamp CreateStandardLamp(Vector3 position, Vector3 color, int sphereId = 0)
        => new(position, color,
            ambient: new Vector3(0.05f, 0.05f, 0.05f),
            diffuse: new Vector3(0.8f, 0.8f, 0.8f),
            specular: new Vector3(1f, 1f, 1f),
            constant: 1f,
            linear: 0.35f,
            quadratic: 0.44f,
            sphereId);

    /// <summary>
    /// Renders the lamp.
    /// </summary>
    /// <param name="shader">The shader used for rendering.</param>
    /// <param name="scale">The scale of the scene.</param>
    /// <param name="curve">The curvature of the scene.</param>
    /// <param name="cameraPosition">The camera position in the scene.</param>
    public void Render(Shader shader, float scale, float curve, Vector3 cameraPosition)
    {
        var modelLs = Matrix4.CreateTranslation(
            GeomPorting.CreateTranslationTarget(Position, cameraPosition, curve, scale));
        var scaleLs = Matrix4.CreateScale(scale);
        shader.SetMatrix4("model", scaleLs * modelLs);
        shader.SetVector3("color", Color);

        GL.BindVertexArray(_sphereResource.Vaos[0]);
        GL.DrawElements(PrimitiveType.Triangles, _sphereResource.Model.Meshes[0].FaceCount * 3,
            DrawElementsType.UnsignedInt, 0);
    }
}
