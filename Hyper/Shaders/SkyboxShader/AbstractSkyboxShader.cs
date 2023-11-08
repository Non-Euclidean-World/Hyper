using Common;
using Hyper.PlayerData;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Hyper.Shaders.SkyboxShader;
internal abstract class AbstractSkyboxShader : Shader
{
    public float GlobalScale { get; }

    private static readonly (string path, ShaderType shaderType)[] ShaderInfo = new[]
    {
        ("Shaders/skybox_shader.vert", ShaderType.VertexShader),
        ("Shaders/skybox_shader.frag", ShaderType.FragmentShader)
    };

    protected AbstractSkyboxShader(float scale) : base(ShaderInfo)
    {
        GlobalScale = scale;
    }

    private void SetView(Matrix4 view) => SetMatrix4("view", view);

    private void SetProjection(Matrix4 projection) => SetMatrix4("projection", projection);

    public virtual void SetUp(Camera camera)
    {
        Use();
        var view = new Matrix4(new Matrix3(camera.GetViewMatrix()));

        SetInt("skybox", 0);
        SetView(view);
        SetProjection(camera.GetProjectionMatrix());
    }
}
