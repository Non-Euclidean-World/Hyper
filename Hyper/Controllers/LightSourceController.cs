using Common;
using Hyper.Shaders;

namespace Hyper.Controllers;

// TODO There should be no separate light source controller. Those should be parts of other bodies.
internal class LightSourceController : IController
{
    private readonly Scene _scene;

    private readonly Shader _shader;

    public LightSourceController(Scene scene, Shader shader)
    {
        _scene = scene;
        _shader = shader;
    }

    public void Render()
    {
        ShaderFactory.SetUpLightingShaderParams(_shader, _scene.Camera);

        foreach (var light in _scene.LightSources)
        {
            light.Render(_shader, _scene.Scale, _scene.Camera.ReferencePointPosition);
        }
    }
}