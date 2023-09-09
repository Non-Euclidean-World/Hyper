using Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Hyper.Shaders;

public class ObjectShader : Shader
{
    private ObjectShader((string path, ShaderType shaderType)[] shaders) : base(shaders) { }

    public static ObjectShader Create()
    {
        var shader = new[]
        {
            ("Shaders/lighting_shader.vert", ShaderType.VertexShader),
            ("Shaders/lighting_shader.frag", ShaderType.FragmentShader)
        };

        return new ObjectShader(shader);
    }

    public void SetCurv(float curv) => SetFloat("curv", curv);

    public void SetAnti(float anti) => SetFloat("anti", 1.0f);

    public void SetView(Matrix4 view) => SetMatrix4("view", view);

    public void SetProjection(Matrix4 projection) => SetMatrix4("projection", projection);

    public void SetNumLights(int numLights) => SetInt("numLights", numLights);

    public void SetViewPos(Vector4 viewPos) => SetVector4("viewPos", viewPos);

    public void SetLightColors(Vector3[] lightColors) => SetVector3Array("lightColor", lightColors);

    public void SetLightPositions(Vector4[] lightPositions) => SetVector4Array("lightPos", lightPositions);
}