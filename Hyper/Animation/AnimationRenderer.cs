using Assimp;
using OpenTK.Graphics.OpenGL4;

namespace Hyper.Animation;

internal class AnimationRenderer
{
    private readonly Shader _shader;

    public AnimationRenderer()
    {
        _shader = CreateShader();
    }
    public void Render(Model model)
    {
        
    }

    private Shader CreateShader()
    {
        var shader = new (string, ShaderType)[]
        {
            ("Animation/Shaders/shader2d.vert", ShaderType.VertexShader),
            ("Animation/Shaders/shader2d.frag", ShaderType.FragmentShader)
        };

        return new Shader(shader);
    }
}