using Common.UserInput;
using Hyper.Shaders.LightSourceShader;
using Hyper.Shaders.ModelShader;
using Hyper.Shaders.ObjectShader;
using Hyper.Transporters;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Hyper.Controllers;

internal class VehiclesController : IController, IInputSubscriber
{
    private readonly Scene _scene;

    private readonly AbstractObjectShader _objectShader;

    private readonly AbstractModelShader _modelShader;

    private readonly AbstractLightSourceShader _lightSourceShader;

    private readonly ITransporter _transporter;

    public VehiclesController(Scene scene, Context context, AbstractObjectShader objectShader, AbstractLightSourceShader lightSourceShader, AbstractModelShader modelShader, ITransporter transporter)
    {
        _scene = scene;
        _objectShader = objectShader;
        _lightSourceShader = lightSourceShader;
        _modelShader = modelShader;
        RegisterCallbacks(context);
        _transporter = transporter;
    }

    public void Render()
    {

        foreach (var car in _scene.FreeCars)
        {
            _objectShader.SetUp(_scene.Camera, _scene.LightSources, car.CurrentSphereId);
            car.Mesh.Render(_objectShader, _objectShader.GlobalScale, _scene.Camera.Curve, _scene.Camera.ReferencePointPosition);
        }

        if (_scene.PlayersCar != null)
        {
            _objectShader.SetUp(_scene.Camera, _scene.LightSources, _scene.PlayersCar.CurrentSphereId);
            _scene.PlayersCar.Mesh.Render(_objectShader, _objectShader.GlobalScale, _scene.Camera.Curve, _scene.Camera.ReferencePointPosition);
        }
    }

    public void RegisterCallbacks(Context context)
    {
        context.RegisterKeys(new List<Keys> { Keys.LeftShift, Keys.Space, Keys.W, Keys.S, Keys.A, Keys.D, Keys.L, Keys.Space });
        context.RegisterUpdateFrameCallback((e) =>
        {
            if (_scene.PlayersCar != null)
            {
                float steeringSum = 0;
                if (context.HeldKeys[Keys.A]) steeringSum += 1;
                if (context.HeldKeys[Keys.D]) steeringSum -= 1;
                float targetSpeedFraction = context.HeldKeys[Keys.W] ? 1f : context.HeldKeys[Keys.S] ? -1f : 0;
                _scene.PlayersCar.Update(_scene.SimulationManager.Simulation, (float)e.Time, steeringSum, targetSpeedFraction, context.HeldKeys[Keys.LeftShift], context.HeldKeys[Keys.Space]);

                _scene.Camera.UpdateWithCar(_scene.PlayersCar);

                int targetSphereId = 1 - _scene.PlayersCar.CurrentSphereId;
                if (_transporter.TryTeleportCarTo(targetSphereId, _scene.PlayersCar, _scene.SimulationManager.Simulation, out var exitPoint))
                {
                    _transporter.UpdateCamera(targetSphereId, _scene.Camera, exitPoint);
                    _objectShader.SetInt("characterSphere", targetSphereId);
                    _modelShader.SetInt("characterSphere", targetSphereId);
                    _lightSourceShader.SetInt("characterSphere", targetSphereId);
                }
            }

            foreach (var car in _scene.FreeCars)
            {
                car.Update(_scene.SimulationManager.Simulation, (float)e.Time, targetSteeringAngle: 0f, targetSpeedFraction: 0f, zoom: false, brake: false);
            }
        });

        context.RegisterKeyDownCallback(Keys.L, () =>
        {
            _scene.LeaveCar();
        });
    }

    public void Dispose()
    {
        _objectShader.Dispose();
    }
}