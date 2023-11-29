using Character.Vehicles;
using Common.UserInput;
using Hyper.Controllers.Bots.Spawn;
using Hyper.PlayerData;
using Hyper.Shaders.LightSourceShader;
using Hyper.Shaders.ModelShader;
using Hyper.Shaders.ObjectShader;
using Hyper.Transporters;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Physics.TypingUtils;

namespace Hyper.Controllers;

internal class VehiclesController : IController, IInputSubscriber
{
    private readonly Scene _scene;

    private readonly AbstractObjectShader _objectShader;

    private readonly AbstractModelShader _modelShader;

    private readonly AbstractLightSourceShader _lightSourceShader;

    private readonly ITransporter _transporter;

    private readonly AbstractBotSpawnStrategy _spawnStrategy;

    public VehiclesController(Scene scene, Context context, AbstractObjectShader objectShader,
        AbstractLightSourceShader lightSourceShader, AbstractModelShader modelShader, ITransporter transporter,
        AbstractBotSpawnStrategy spawnStrategy)
    {
        _scene = scene;
        _objectShader = objectShader;
        _lightSourceShader = lightSourceShader;
        _modelShader = modelShader;
        _transporter = transporter;
        _spawnStrategy = spawnStrategy;
        RegisterCallbacks(context);
        _spawnStrategy.Spawn();
    }

    public void Render()
    {
        foreach (var car in _scene.FreeCars)
        {
            _modelShader.SetUp(_scene.Camera, _scene.LightSources, _scene.FlashLights, shininess: 32, car.CurrentSphereId);
            _modelShader.SetBool("isAnimated", false);
            car.Render(_modelShader, _objectShader.GlobalScale, _scene.Camera.Curve, _scene.Camera.ReferencePointPosition, _scene.SimulationManager.Simulation.Bodies);
        }

        if (_scene.PlayersCar != null)
        {
            _modelShader.SetUp(_scene.Camera, _scene.LightSources, _scene.FlashLights, shininess: 32, _scene.PlayersCar.CurrentSphereId);
            _modelShader.SetBool("isAnimated", false);
            _scene.PlayersCar.Render(_modelShader, _objectShader.GlobalScale, _scene.Camera.Curve, _scene.Camera.ReferencePointPosition, _scene.SimulationManager.Simulation.Bodies);
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
                if (_scene.PlayersCar.CurrentSphereId == 0)
                {
                    if (context.HeldKeys[Keys.A]) steeringSum += 1;
                    if (context.HeldKeys[Keys.D]) steeringSum -= 1;
                }
                else
                {
                    if (context.HeldKeys[Keys.A]) steeringSum -= 1;
                    if (context.HeldKeys[Keys.D]) steeringSum += 1;
                }

                float targetSpeedFraction = context.HeldKeys[Keys.W] ? 1f : context.HeldKeys[Keys.S] ? -1f : 0;
                _scene.PlayersCar.Update(_scene.SimulationManager.Simulation, (float)e.Time, steeringSum, targetSpeedFraction, context.HeldKeys[Keys.LeftShift], context.HeldKeys[Keys.Space]);

                UpdateCamera(_scene.Camera, _scene.PlayersCar);

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

                int targetSphereId = 1 - car.CurrentSphereId;
                _transporter.TryTeleportCarTo(targetSphereId, car, _scene.SimulationManager.Simulation, out var _);
            }
        });

        context.RegisterKeyDownCallback(Keys.L, _scene.LeaveCar);
        context.RegisterKeyDownCallback(Keys.Y, () => _scene.PlayersCar?.Lights.ForEach(x => x.Active = !x.Active));
    }

    private void UpdateCamera(Camera camera, FourWheeledCar car)
    {
        if (camera.Sphere == 0)
        {
            camera.ReferencePointPosition = Conversions.ToOpenTKVector(car.CarBodyPose.Position)
               + (camera.FirstPerson ? GetFirstPersonCameraOffset(camera, car) : GetThirdPersonCameraOffset(camera))
               - (camera.Curve > 0 ? camera.SphereCenter : Vector3.Zero);
        }
        else
        {
            var playerCarPos = Conversions.ToOpenTKVector(car.CarBodyPose.Position);
            playerCarPos.Y *= -1;
            camera.ReferencePointPosition = playerCarPos
                + (camera.FirstPerson ? GetFirstPersonCameraOffset(camera, car) : GetThirdPersonCameraOffset(camera))
                - (camera.Curve > 0 ? camera.SphereCenter : Vector3.Zero);
        }
    }

    private Vector3 GetThirdPersonCameraOffset(Camera camera)
        => camera.Up * 1f - camera.Front * 5f;

    private Vector3 GetFirstPersonCameraOffset(Camera camera, FourWheeledCar car)
        => camera.Up * 0.4f - 0.2f * camera.Front
            * System.Numerics.Vector3.Distance(car.SimpleCar.BackLeftWheel.BodyToWheelSuspension, car.SimpleCar.FrontLeftWheel.BodyToWheelSuspension);

    public void Dispose()
    {
        _objectShader.Dispose();
    }
}