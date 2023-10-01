using Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Character.Shaders;

public class ModelShader : Shader
{
    private ModelShader((string path, ShaderType shaderType)[] shaders) : base(shaders) { }

    public static ModelShader Create()
    {
        var shader = new[]
        {
            ("Shaders/model_shader.vert", ShaderType.VertexShader),
            ("Shaders/model_shader.frag", ShaderType.FragmentShader)
        };

        return new ModelShader(shader);
    }

    public void SetCurv(float curv) => SetFloat("curv", curv);

    public void SetView(Matrix4 view) => SetMatrix4("view", view);

    public void SetProjection(Matrix4 projection) => SetMatrix4("projection", projection);

    public void SetNumLights(int numLights) => SetInt("numLights", numLights);

    public void SetViewPos(Vector4 viewPos) => SetVector4("viewPos", viewPos);

    public void SetLightColors(Vector3[] lightColors) => SetVector3Array("lightColor", lightColors);

    public void SetLightPositions(Vector4[] lightPositions) => SetVector4Array("lightPos", lightPositions);

    public void SetLowerSphereCenter(Vector3 lowerSphereCenter) => SetVector3("lowerSphereCenter", lowerSphereCenter);

    public void SetSphere(int sphere) => SetInt("sphere", sphere);
}