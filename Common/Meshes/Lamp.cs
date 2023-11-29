using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Common.Meshes;

/// <summary>
/// Class representing a point light source
/// </summary>
public class Lamp : Mesh
{
    public Vector3 Color { get; set; }
    public Vector3 Ambient { get; private init; }
    public Vector3 Diffuse { get; private init; }
    public Vector3 Specular { get; private init; }
    public float Constant { get; private init; }
    public float Linear { get; private init; }
    public float Quadratic { get; private init; }

    public Lamp(Vertex[] vertices, Vector3 position, Vector3 color, Vector3 ambient, Vector3 diffuse, Vector3 specular, float constant, float linear, float quadratic) : base(vertices, position)
    {
        Color = color;
        Ambient = ambient;
        Diffuse = diffuse;
        Specular = specular;
        Constant = constant;
        Linear = linear;
        Quadratic = quadratic;
    }

    /// <summary>
    /// Creates a glowing cube
    /// </summary>
    /// <param name="position"></param>
    /// <param name="color"></param>
    /// <returns></returns>
    public static Lamp CreateStandardLamp(Vector3 position, Vector3 color)
        => new(CubeMesh.Vertices, position, color,
            ambient: new Vector3(0.05f, 0.05f, 0.05f),
            diffuse: new Vector3(0.8f, 0.8f, 0.8f),
            specular: new Vector3(1f, 1f, 1f),
            constant: 1f,
            linear: 0.35f,
            quadratic: 0.44f);

    public override void Render(Shader shader, float scale, float curve, Vector3 cameraPosition)
    {
        var modelLs = Matrix4.CreateTranslation(
            GeomPorting.CreateTranslationTarget(Position, cameraPosition, curve, scale));
        var scaleLs = Matrix4.CreateScale(scale);
        shader.SetMatrix4("model", scaleLs * modelLs);
        shader.SetVector3("color", Color);

        GL.BindVertexArray(VaoId);
        GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
    }
}
