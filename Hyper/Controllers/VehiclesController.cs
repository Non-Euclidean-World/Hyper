using Common.UserInput;
using Hyper.Shaders.ObjectShader;
using OpenTK.Windowing.GraphicsLibraryFramework;

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

        foreach (var car in _scene.BotCars)
        {
            car.Mesh.Render(_shader, _shader.GlobalScale, _scene.Camera.Curve, _scene.Camera.ReferencePointPosition);
        }

        _scene.PlayersCar.Mesh.Render(_shader, _shader.GlobalScale, _scene.Camera.Curve, _scene.Camera.ReferencePointPosition);
    }

    public void RegisterCallbacks(Context context)
    {
        context.RegisterKeys(new List<Keys> { Keys.LeftShift, Keys.Space, Keys.W, Keys.S, Keys.A, Keys.D, Keys.L, Keys.Space });
        context.RegisterUpdateFrameCallback((e) =>
        {
            foreach (var car in _scene.BotCars)
            {
                car.Update(_scene.SimulationManager.Simulation, (float)e.Time, 0, 0f, false, false);
            }
            if (context.Mode == Context.InputMode.PlayerInCar)
            {
                float steeringSum = 0;
                if (context.HeldKeys[Keys.A]) steeringSum += 1;
                if (context.HeldKeys[Keys.D]) steeringSum -= 1;
                float targetSpeedFraction = context.HeldKeys[Keys.W] ? 1f : context.HeldKeys[Keys.S] ? -1f : 0;
                _scene.PlayersCar.Update(_scene.SimulationManager.Simulation, (float)e.Time, steeringSum, targetSpeedFraction, context.HeldKeys[Keys.LeftShift], context.HeldKeys[Keys.Space]);

                _scene.Camera.UpdateWithCar(_scene.PlayersCar);
            }
            else
            {
                _scene.PlayersCar.Update(_scene.SimulationManager.Simulation, (float)e.Time, 0, 0f, false, false);
            }
        });

        context.RegisterKeyDownCallback(Keys.L, () => context.Mode = Context.InputMode.PlayerOnFoot);
    }

    public void Dispose()
    {
        _shader.Dispose();
    }
}