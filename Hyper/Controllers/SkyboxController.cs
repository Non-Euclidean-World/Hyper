using Hyper.Shaders.SkyboxShader;

namespace Hyper.Controllers;
internal class SkyboxController : IController
{
    private readonly Scene _scene;

    private readonly AbstractSkyboxShader _skyboxShader;

    private readonly Skybox.Skybox _skybox;

    public SkyboxController(Scene scene, AbstractSkyboxShader skyboxShader)
    {
        _scene = scene;
        _skyboxShader = skyboxShader;
        _skybox = new Skybox.Skybox(skyboxShader.GlobalScale);
    }

    public void Dispose()
    {
        _skyboxShader.Dispose();
    }

    public void Render()
    {
        _skyboxShader.SetUp(_scene.Camera);
        _skybox.Render(_skyboxShader);
    }
}
