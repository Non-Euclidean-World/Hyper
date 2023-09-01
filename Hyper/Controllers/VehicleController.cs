using Common;
using Common.UserInput;
using Hyper.Shaders;

namespace Hyper.Controllers;

internal class VehicleController : IController, IInputSubscriber
{
    private readonly Scene _scene;

    private readonly Shader _shader;

    public VehicleController(Scene scene, Shader shader)
    {
        _scene = scene;
        _shader = shader;
        RegisterCallbacks();
    }
    
    public void Render()
    {
        ShaderFactory.SetUpObjectShaderParams(_shader, _scene.Camera, _scene.LightSources, _scene.Scale);
        
        _scene.SimpleCar.Mesh.Render(_shader, _scene.Scale, _scene.Camera.ReferencePointPosition);
    }

    public void RegisterCallbacks()
    {
        var context = Context.Instance;
        context.RegisterUpdateFrameCallback((e) => 
            _scene.SimpleCar.Update(_scene.SimulationManager.Simulation, (float)e.Time, 0, 0f, false, false));
    }
}