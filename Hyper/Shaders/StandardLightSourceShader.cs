using OpenTK.Graphics.OpenGL4;

namespace Hyper.Shaders;
public class StandardLightSourceShader : AbstractLightSourceShader
{
    private StandardLightSourceShader((string path, ShaderType shaderType)[] shaders, float globalScale) : base(shaders, globalScale)
    { }

    public static StandardLightSourceShader Create(float globalScale)
    {
        var shader = new[]
        {
            ("Shaders/lighting_shader.vert", ShaderType.VertexShader),
            ("Shaders/light_source_shader.frag", ShaderType.FragmentShader)
        };

        return new StandardLightSourceShader(shader, globalScale);
    }
}
