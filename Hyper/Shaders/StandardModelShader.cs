using Hyper.Shaders;
using OpenTK.Graphics.OpenGL4;

namespace Character.Shaders;
internal class StandardModelShader : AbstractModelShader
{
    private StandardModelShader((string path, ShaderType shaderType)[] shaders, float globalScale)
        : base(shaders, globalScale)
    { }

    public static StandardModelShader Create(float globalScale)
    {
        var shader = new[]
        {
            ("Shaders/model_shader.vert", ShaderType.VertexShader),
            ("Shaders/model_shader.frag", ShaderType.FragmentShader)
        };

        return new StandardModelShader(shader, globalScale);
    }
}
