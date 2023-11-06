using Common;
using Common.Meshes;
using Hyper.PlayerData;
using Hyper.Shaders.DataTypes;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Hyper.Shaders.ObjectShader;
internal abstract class AbstractObjectShader : Shader
{
    public float GlobalScale { get; private init; }

    private static readonly (string path, ShaderType shaderType)[] ShaderInfo = new[]
        {
            ("Shaders/lighting_shader.vert", ShaderType.VertexShader),
            ("Shaders/lighting_shader.frag", ShaderType.FragmentShader)
        };

    protected AbstractObjectShader(float globalScale)
        : base(ShaderInfo)
    {
        GlobalScale = globalScale;
    }

    private void SetCurv(float curv) => SetFloat("curv", curv);

    private void SetView(Matrix4 view) => SetMatrix4("view", view);

    private void SetProjection(Matrix4 projection) => SetMatrix4("projection", projection);

    private void SetNumLights(int numLights) => SetInt("numLights", numLights);

    private void SetViewPos(Vector4 viewPos) => SetVector4("viewPos", viewPos);

    private void SetPointLights(PointLight[] lights) => SetStructArray("lightCasters", lights);

    public virtual void SetUp(Camera camera, List<LightSource> lightSources, int sphere = 0)
    {
        Use();
        SetCurv(camera.Curve);
        SetView(camera.GetViewMatrix());
        SetProjection(camera.GetProjectionMatrix());
        SetNumLights(lightSources.Count);
        SetViewPos(GeomPorting.EucToCurved(camera.ViewPosition, camera.Curve));

        SetPointLights(lightSources.Select(x =>
           new PointLight
           {
               Position = GeomPorting.EucToCurved(GeomPorting.CreateTranslationTarget(x.Position, camera.ReferencePointPosition, camera.Curve, GlobalScale), camera.Curve),
               Color = x.Color,
               Ambient = x.Ambient,
               Diffuse = x.Diffuse,
               Specular = x.Specular,
               Constant = x.Constant,
               Linear = x.Linear,
               Quadratic = x.Quadratic,
           }
        ).ToArray());
    }
}
