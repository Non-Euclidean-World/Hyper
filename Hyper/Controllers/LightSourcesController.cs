using Hyper.Shaders.LightSourceShader;

namespace Hyper.Controllers;

// TODO There should be no separate light source controller. Those should be parts of other bodies.
internal class LightSourcesController : IController
{
    private readonly Scene _scene;

    private readonly AbstractLightSourceShader _shader;

    public LightSourcesController(Scene scene, AbstractLightSourceShader shader)
    {
        _scene = scene;
        _shader = shader;
    }

    public void Render()
    {
        foreach (var light in _scene.LightSources)
        {
            _shader.SetUp(_scene.Camera, light.CurrentSphereId);
            light.Render(_shader, _scene.GlobalScale, _scene.Camera.Curve, _scene.Camera.ReferencePointPosition);
        }
    }

    public void Dispose()
    {
        _shader.Dispose();
    }
}