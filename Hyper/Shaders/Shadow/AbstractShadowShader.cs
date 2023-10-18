using System.Data;
using Common;
using Common.Meshes;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Hyper.Shaders.Shadow;

public abstract class AbstractShadowShader : Shader
{
    public float GlobalScale { get; private init; }

    private static readonly (string path, ShaderType shaderType)[] ShaderInfo = new[]
    {
        ("Shaders/Shadow/shader.vert", ShaderType.VertexShader),
        ("Shaders/Shadow/shader.frag", ShaderType.FragmentShader),
        ("Shaders/Shadow/shader.geom", ShaderType.GeometryShader)
    };

    protected AbstractShadowShader(float globalScale) : base(ShaderInfo)
    {
        GlobalScale = globalScale;
    }
    
    // TODO change the names to match c# conventions
    public void SetTransforms(Matrix4[] transforms) => SetMatrix4Array("shadowMatrices", transforms);
    
    public void SetFarPlane(float farPlane) => SetFloat("far_plane", farPlane);
    
    public void SetLightPos(Vector3 lightPos) => SetVector3("lightPos", lightPos);
    
    public void SetModel(Matrix4 model) => SetMatrix4("model", model);

    public void SetUp(LightSource lightSource)
    {
        Use();
        SetTransforms(lightSource.GetShadowTransforms());
        SetLightPos(lightSource.Position);
        SetFarPlane(LightSource.FarPlane);
    }
}