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

    private void SetNumPointLights(int numPointLights) => SetInt("numPointLights", numPointLights);

    private void SetNumSpotLights(int numSpotLights) => SetInt("numSpotLights", numSpotLights);

    private void SetViewPos(Vector4 viewPos) => SetVector4("viewPos", viewPos);

    private void SetPointLights(PointLight[] lights) => SetStructArray("pointLights", lights);

    private void SetSpotLights(DataTypes.SpotLight[] lights) => SetStructArray("spotLights", lights);

    public virtual void SetUp(Camera camera, List<Lamp> lightSources, List<PlayerData.FlashLight> flashLights, int sphere = 0)
    {
        Use();
        SetCurv(camera.Curve);
        SetView(camera.GetViewMatrix());
        SetProjection(camera.GetProjectionMatrix());
        SetNumPointLights(lightSources.Count);
        SetNumSpotLights(flashLights.Count);
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

        SetSpotLights(flashLights.Select(x =>
        new SpotLight
        {
            Position = GeomPorting.EucToCurved(GeomPorting.CreateTranslationTarget(x.Position, camera.ReferencePointPosition, camera.Curve, GlobalScale), camera.Curve),
            Color = x.Color,
            Direction = new Vector4(x.Direction, 0),
            CutOff = x.CutOff,
            OuterCutOff = x.OuterCutOff,
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
