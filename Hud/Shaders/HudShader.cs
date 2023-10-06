using Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Hud.Shaders;

public class HudShader : Shader
{
    private HudShader((string path, ShaderType shaderType)[] shaders) : base(shaders) { }

    public static HudShader Create()
    {
        var shader = new[]
        {
            ("Shaders/shader2d.vert", ShaderType.VertexShader),
            ("Shaders/shader2d.frag", ShaderType.FragmentShader)
        };

        return new HudShader(shader);
    }

    public void SetProjection(Matrix4 projection) => SetMatrix4("projection", projection);

    public void SetUp(float aspectRatio)
    {
        Use();
        var projection = Matrix4.CreateOrthographic(aspectRatio, 1, -1.0f, 1.0f);
        SetProjection(projection);
    }
}