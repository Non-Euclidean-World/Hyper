using OpenTK.Graphics.OpenGL4;

namespace Hyper.Shaders;
public class StandardObjectShader : AbstractObjectShader
{
    private StandardObjectShader((string path, ShaderType shaderType)[] shaders, float globalScale) : base(shaders, globalScale)
    {
    }

    public static StandardObjectShader Create(float globalScale)
    {
        var shader = new[]
        {
            ("Shaders/lighting_shader.vert", ShaderType.VertexShader),
            ("Shaders/lighting_shader.frag", ShaderType.FragmentShader)
        };

        return new StandardObjectShader(shader, globalScale);
    }
}
