using Character.LightSources;
using Common;
using Common.Meshes;
using Hyper.PlayerData;
using Hyper.Shaders.Helpers;
using OpenTK.Graphics.OpenGL4;

namespace Hyper.Shaders.ModelShader;
internal class AbstractModelShader : Shader
{
    private static readonly (string path, ShaderType shaderType)[] ShaderInfo = new[]
        {
            ("Shaders/model_shader.vert", ShaderType.VertexShader),
            ("Shaders/model_shader.frag", ShaderType.FragmentShader)
        };

    protected AbstractModelShader()
        : base(ShaderInfo)
    {
    }

    private void SetShininess(float shininess) => SetFloat("shininess", shininess);

    public virtual void SetUp(Camera camera, List<Lamp> lightSources, List<FlashLight> flashLights, float shininess, float globalScale, int sphere = 0)
    {
        Use();

        ShaderData.SetUpTransforms(this, camera);
        ShaderData.SetUpLighting(this, lightSources, flashLights, camera, globalScale);
        SetShininess(shininess); // TODO this should be property of a material

        SetBool("isAnimated", true);
    }
}
