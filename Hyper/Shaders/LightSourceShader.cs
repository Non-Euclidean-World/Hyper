using Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Hyper.Shaders;

public class LightSourceShader : Shader
{
    private LightSourceShader((string path, ShaderType shaderType)[] shaders) : base(shaders) { }

    public static LightSourceShader Create()
    {
        var shader = new[]
        {
            ("Shaders/lighting_shader.vert", ShaderType.VertexShader),
            ("Shaders/light_source_shader.frag", ShaderType.FragmentShader)
        };

        return new LightSourceShader(shader);
    }

    public void SetCurv(float curv) => SetFloat("curv", curv);

    public void SetAnti(float anti) => SetFloat("anti", 1.0f);

    public void SetView(Matrix4 view) => SetMatrix4("view", view);

    public void SetProjection(Matrix4 projection) => SetMatrix4("projection", projection);
}