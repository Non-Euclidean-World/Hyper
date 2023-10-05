using Common.UserInput;
using Hyper.Shaders;

namespace Hyper.Controllers;

internal class VehiclesController : IController, IInputSubscriber
{
    private readonly Scene _scene;

    private readonly AbstractObjectShader _shader;

    public VehiclesController(Scene scene, Context context, AbstractObjectShader shader)
    {
        _scene = scene;
        _shader = shader;
        RegisterCallbacks(context);
    }

    public void Render()
    {
        _shader.SetUp(_scene.Camera, _scene.LightSources, sphere: 0); // TODO current sphere

        foreach (var car in _scene.Cars)
        {
            car.Mesh.Render(_shader, _shader.GlobalScale, _scene.Camera.Curve, _scene.Camera.ReferencePointPosition);
        }
    }

    public void RegisterCallbacks(Context context)
    {
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