using OpenTK.Graphics.OpenGL4;

namespace Hyper.Animation;

public class Renderer
{
    private readonly Shader _shader;

    public Renderer()
    {
        _shader = CreateShader();
    }
    
    private Shader CreateShader()
    {
        var shader = new (string, ShaderType)[]
        {
            ("HUD/Shaders/shader2d.vert", ShaderType.VertexShader),
            ("HUD/Shaders/shader2d.frag", ShaderType.FragmentShader)
        };

        return new Shader(shader);
    }
}