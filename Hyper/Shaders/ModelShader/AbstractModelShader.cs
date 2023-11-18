using Common;
using Common.Meshes;
using Hyper.PlayerData;
using Hyper.PlayerData.Utils;
using Hyper.Shaders.DataTypes;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Hyper.Shaders.ModelShader;
internal class AbstractModelShader : Shader
{
    public float GlobalScale { get; private init; }

    private static readonly (string path, ShaderType shaderType)[] ShaderInfo = new[]
        {
            ("Shaders/model_shader.vert", ShaderType.VertexShader),
            ("Shaders/model_shader.frag", ShaderType.FragmentShader)
        };

    protected AbstractModelShader(float globalScale)
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

    private void SetSpotLights(SpotLight[] lights) => SetStructArray("spotLights", lights);

    private void SetShininess(float shininess) => SetFloat("shininess", shininess);

    public virtual void SetUp(Camera camera, List<Lamp> lightSources, List<FlashLight> flashLights, float shininess, int sphere = 0)
    {
        Use();
        SetCurv(camera.Curve);
        SetView(camera.GetViewMatrix());
        SetProjection(camera.GetProjectionMatrix());

        SetNumPointLights(lightSources.Count);
        SetNumSpotLights(flashLights.Where(x => x.Active).Count());
        SetShininess(shininess);
        SetViewPos(GeomPorting.EucToCurved(camera.ViewPosition, camera.Curve)); // shouldnt be referencepointpos?
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

        SetSpotLights(flashLights
            .Where(x => x.Active)
            .Select(x =>
            new SpotLight
            {
                Position = GeomPorting.EucToCurved(GeomPorting.CreateTranslationTarget(x.Position, camera.ReferencePointPosition, camera.Curve, GlobalScale), camera.Curve),
                Color = x.Color,
                Direction = new Vector4(x.Direction, 0) * Matrices.TranslationMatrix(GeomPorting.EucToCurved(GeomPorting.CreateTranslationTarget(x.Position, camera.ReferencePointPosition, camera.Curve, GlobalScale), camera.Curve), camera.Curve),
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

        SetBool("isAnimated", true);
    }
}
