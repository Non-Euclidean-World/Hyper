
using Common;
using Common.Meshes;
using Hyper.PlayerData;
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

    public void SetCurv(float curv) => SetFloat("curv", curv);

    public void SetView(Matrix4 view) => SetMatrix4("view", view);

    public void SetProjection(Matrix4 projection) => SetMatrix4("projection", projection);

    public void SetNumLights(int numLights) => SetInt("numLights", numLights);

    public void SetViewPos(Vector4 viewPos) => SetVector4("viewPos", viewPos);

    public void SetLightColors(Vector3[] lightColors) => SetVector3Array("lightColor", lightColors);

    public void SetLightPositions(Vector4[] lightPositions) => SetVector4Array("lightPos", lightPositions);

    public virtual void SetUp(Camera camera, List<LightSource> lightSources, int sphere = 0)
    {
        Use();
        SetCurv(camera.Curve);
        SetView(camera.GetViewMatrix());
        SetProjection(camera.GetProjectionMatrix());

        SetNumLights(lightSources.Count);
        SetViewPos(GeomPorting.EucToCurved(camera.ViewPosition, camera.Curve));
        SetLightColors(lightSources.Select(x => x.Color).ToArray());
        SetLightPositions(lightSources.Select(x =>
            GeomPorting.EucToCurved(GeomPorting.CreateTranslationTarget(x.Position, camera.ReferencePointPosition, camera.Curve, GlobalScale), camera.Curve)).ToArray());

        SetBool("isAnimated", true);
    }
}
