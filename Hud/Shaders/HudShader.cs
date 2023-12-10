using Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Hud.Shaders;
/// <summary>
/// A shader that is used to render 2D elements.
/// </summary>
public class HudShader : Shader
{
    private HudShader((string path, ShaderType shaderType)[] shaders) : base(shaders) { }

    /// <summary>
    /// Creates a new instance of the <see cref="HudShader"/> class.
    /// </summary>
    /// <returns>A new <see cref="HudShader"/>.</returns>
    public static HudShader Create()
    {
        var shader = new[]
        {
            ("Shaders/shader2d.vert", ShaderType.VertexShader),
            ("Shaders/shader2d.frag", ShaderType.FragmentShader)
        };

        return new HudShader(shader);
    }

    private void SetProjection(Matrix4 projection) => SetMatrix4("projection", projection);

    /// <summary>
    /// Sets up the shader.
    /// </summary>
    /// <param name="aspectRatio">The screen aspect ratio.</param>
    public void SetUp(float aspectRatio)
    {
        Use();
        var projection = Matrix4.CreateOrthographic(aspectRatio, 1, -1.0f, 1.0f);
        SetProjection(projection);
    }

    /// <summary>
    /// Sets the color of the rendered object.
    /// </summary>
    /// <param name="color"></param>
    public void SetColor(Vector4 color) => SetVector4("color", color);

    /// <summary>
    /// Sets the texture of the rendered object.
    /// </summary>
    /// <param name="useTexture"></param>
    public void UseTexture(bool useTexture) => SetBool("useTexture", useTexture);

    /// <summary>
    /// Sets the model matrix of the rendered object.
    /// </summary>
    /// <param name="model"></param>
    public void SetModel(Matrix4 model) => SetMatrix4("model", model);
}