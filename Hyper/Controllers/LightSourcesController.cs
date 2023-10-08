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
        _shader.SetUp(_scene.Camera, _scene.Player.CurrentSphereId);

        foreach (var light in _scene.LightSources)
        {
            light.Render(_shader, _shader.GlobalScale, _scene.Camera.Curve, _scene.Camera.ReferencePointPosition);
        }
    }

    public void Dispose()
    {
        _shader.Dispose();
    }
}