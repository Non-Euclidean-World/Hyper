using Common;
using Common.Meshes;
using Hyper.PlayerData;
using Hyper.Shaders.Helpers;
using OpenTK.Graphics.OpenGL4;

namespace Hyper.Shaders.ObjectShader;
internal abstract class AbstractObjectShader : Shader
{
    public float GlobalScale { get; private init; }

    private static readonly (string path, ShaderType shaderType)[] ShaderInfo = new[]
        {
            ("Shaders/lighting_shader.vert", ShaderType.VertexShader),
            ("Shaders/lighting_shader.frag", ShaderType.FragmentShader)
        };

    protected AbstractObjectShader(float globalScale)
        : base(ShaderInfo)
    {
        GlobalScale = globalScale;
    }

    private void SetShininess(float shininess) => SetFloat("shininess", shininess);

    public virtual void SetUp(Camera camera, List<Lamp> lightSources, List<FlashLight> flashLights, float shininess, int sphere = 0)
    {
        Use();

        ShaderData.SetUpTransforms(this, camera);
        ShaderData.SetUpLighting(this, lightSources, flashLights, camera, GlobalScale);
        SetShininess(shininess); // TODO this should be property of a material
    }
}
