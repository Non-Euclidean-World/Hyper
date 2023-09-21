using Common.UserInput;
using Hyper.Shaders;

namespace Hyper.Controllers;

internal class VehiclesController : IController, IInputSubscriber
{
    private readonly Scene _scene;

    private readonly ObjectShader _shader;

    public VehiclesController(Scene scene, ObjectShader shader)
    {
        _scene = scene;
        _shader = shader;
        RegisterCallbacks();
    }

    public void Render()
    {
        _shader.SetUp(_scene.Camera, _scene.LightSources, _scene.Scale);

        foreach (var car in _scene.Cars)
        {
            car.Mesh.Render(_shader, _scene.Scale, _scene.Camera.ReferencePointPosition);
        }
    }

    public void RegisterCallbacks()
    {
        var context = Context.Instance;
        context.RegisterUpdateFrameCallback((e) =>
        {
            foreach (var car in _scene.Cars)
            {
                car.Update(_scene.SimulationManager.Simulation, (float)e.Time, 0, 0f, false, false);
            }
        });
    }

    public void Dispose()
    {
        _shader.Dispose();
    }
}