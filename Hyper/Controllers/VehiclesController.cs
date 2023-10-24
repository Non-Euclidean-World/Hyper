using BepuPhysics;
using Character.Vehicles;
using Common;
using Common.UserInput;
using Hyper.Controllers.Bots.Spawn;
using Hyper.PlayerData;
using Hyper.Shaders.LightSourceShader;
using Hyper.Shaders.ModelShader;
using Hyper.Shaders.ObjectShader;
using Hyper.Transporters;
using OpenTK.Graphics.OpenGL4;
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

    private readonly CarBodyResource _bodyResource = CarBodyResource.Instance;

    private readonly CarWheelResource _wheelResource = CarWheelResource.Instance;

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
            _modelShader.SetUp(_scene.Camera, _scene.LightSources, car.CurrentSphereId);
            _modelShader.SetBool("isAnimated", false);
            Render(car);
            RenderWheels(car);
        }

        if (_scene.PlayersCar != null)
        {
            _modelShader.SetUp(_scene.Camera, _scene.LightSources, _scene.PlayersCar.CurrentSphereId);
            _modelShader.SetBool("isAnimated", false);
            Render(_scene.PlayersCar);
            RenderWheels(_scene.PlayersCar);
        }
    }

    private void Render(SimpleCar car, float scale = 2f)
    {
        _bodyResource.Texture.Use(TextureUnit.Texture0);

        var translation = Matrix4.CreateTranslation(
        GeomPorting.CreateTranslationTarget(
                Conversions.ToOpenTKVector(car.CarBodyPose.Position), _scene.Camera.ReferencePointPosition, _scene.Camera.Curve, _objectShader.GlobalScale));

        var rotation = Matrix4.CreateRotationX(-MathF.PI / 2)
            * Conversions.ToOpenTKMatrix(System.Numerics.Matrix4x4.CreateFromQuaternion(car.CarBodyPose.Orientation));
        var globalScale = Matrix4.CreateScale(_objectShader.GlobalScale);
        var localScale = Matrix4.CreateScale(scale);
        _modelShader.SetMatrix4("model", localScale * globalScale * rotation * translation);
        _modelShader.SetMatrix4("normalRotation", rotation);

        for (int i = 0; i < _bodyResource.Model.Meshes.Count; i++)
        {
            GL.BindVertexArray(_bodyResource.Vaos[i]);
            GL.DrawElements(PrimitiveType.Triangles, _bodyResource.Model.Meshes[i].FaceCount * 3,
                DrawElementsType.UnsignedInt, 0);
        }
    }

    private void RenderWheels(SimpleCar car)
    {
        RenderWheel(car.FrontLeftWheel, true);
        RenderWheel(car.FrontRightWheel, false);
        RenderWheel(car.BackLeftWheel, true);
        RenderWheel(car.BackRightWheel, false);
    }

    // TODO make rendering consistent
    // bounding shapes are rendered in controller, cowboy has a Model class & the rendering takes place there
    private void RenderWheel(WheelHandles wheel, bool leftSide, float scale = 2f)
    {
        _wheelResource.Texture.Use(TextureUnit.Texture0);

        var wheelReference = new BodyReference(wheel.Wheel, _scene.SimulationManager.Simulation.Bodies);
        ref var wheelPose = ref wheelReference.Pose;
        var translation = Matrix4.CreateTranslation(
       GeomPorting.CreateTranslationTarget(
               Conversions.ToOpenTKVector(wheelPose.Position), _scene.Camera.ReferencePointPosition, _scene.Camera.Curve, _objectShader.GlobalScale));

        Matrix4 importRotation;
        if (leftSide)
        {
            importRotation = Matrix4.CreateRotationZ(-MathF.PI / 2);
        }
        else
        {
            importRotation = Matrix4.CreateRotationZ(MathF.PI / 2);
        }
        var rotation = importRotation
            * Conversions.ToOpenTKMatrix(System.Numerics.Matrix4x4.CreateFromQuaternion(wheelPose.Orientation));

        var globalScale = Matrix4.CreateScale(_objectShader.GlobalScale);
        var localScale = Matrix4.CreateScale(scale);
        _modelShader.SetMatrix4("model", localScale * globalScale * rotation * translation);
        _modelShader.SetMatrix4("normalRotation", rotation);

        for (int i = 0; i < _wheelResource.Model.Meshes.Count; i++)
        {
            GL.BindVertexArray(_wheelResource.Vaos[i]);
            GL.DrawElements(PrimitiveType.Triangles, _wheelResource.Model.Meshes[i].FaceCount * 3,
                DrawElementsType.UnsignedInt, 0);
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
            }
        });

        context.RegisterKeyDownCallback(Keys.L, () =>
        {
            _scene.LeaveCar();
        });
    }

    private void UpdateCamera(Camera camera, SimpleCar car)
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

    private Vector3 GetFirstPersonCameraOffset(Camera camera, SimpleCar car)
        => camera.Up * 0.4f - 0.2f * camera.Front * System.Numerics.Vector3.Distance(car.BackLeftWheel.BodyToWheelSuspension, car.FrontLeftWheel.BodyToWheelSuspension);

    public void Dispose()
    {
        _objectShader.Dispose();
    }
}