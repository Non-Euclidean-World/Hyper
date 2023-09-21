using Hyper.Shaders;

namespace Hyper.Controllers;

// TODO There should be no separate light source controller. Those should be parts of other bodies.
internal class LightSourcesController : IController
{
    private readonly Scene _scene;

    private readonly LightSourceShader _shader;

    public LightSourcesController(Scene scene, LightSourceShader shader)
    {
        _scene = scene;
        _shader = shader;
    }

    public void Render()
    {
        _shader.SetUp(_scene.Camera);

        foreach (var light in _scene.LightSources)
        {
            light.Render(_shader, _scene.Scale, _scene.Camera.ReferencePointPosition);
        }
    }
    
    public void Dispose()
    {
        _shader.Dispose();
    }
}