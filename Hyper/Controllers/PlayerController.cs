using Character.Shaders;
using Common.UserInput;
using Hyper.Shaders;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Hyper.Controllers;

internal class PlayerController : IController, IInputSubscriber
{
    private readonly Scene _scene;

    private readonly ModelShader _modelShader;

    private readonly ObjectShader _objectShader;

    private readonly LightSourceShader _rayMarkerShader;

    private readonly SphericalTransporter _sphericalTransporter;

    private bool _showBoundingBoxes = false;

    private bool _spherical;

    public PlayerController(Scene scene, Context context, ModelShader modelShader, ObjectShader objectShader, LightSourceShader rayMarkerShader, SphericalTransporter sphericalTransporter, bool spherical)
    {
        _scene = scene;
        _modelShader = modelShader;
        _objectShader = objectShader;
        _rayMarkerShader = rayMarkerShader;
        _sphericalTransporter = sphericalTransporter;
        _spherical = spherical;
        RegisterCallbacks(context);
    }

    public void Render()
    {
        _modelShader.SetUp(_scene.Camera, _scene.LightSources, _scene.Scale, _scene.Player.CurrentSphereId, _scene.LowerSphereCenter);
        _scene.Player.Render(_modelShader, _scene.Scale, _scene.Camera.Curve, _scene.Camera.ReferencePointPosition, _scene.Camera.FirstPerson);

        _rayMarkerShader.SetUp(_scene.Camera, _scene.Player.CurrentSphereId, _scene.LowerSphereCenter);
        _scene.Player.RenderRay(in _scene.SimulationManager.RayCastingResults[_scene.Player.RayId], _rayMarkerShader, _scene.Scale, _scene.Camera.Curve, _scene.Camera.ReferencePointPosition);

        if (!_showBoundingBoxes)
            return;

        _objectShader.SetUp(_scene.Camera, _scene.LightSources, _scene.Scale, _scene.Player.CurrentSphereId, _scene.LowerSphereCenter);
        _scene.Player.PhysicalCharacter.RenderBoundingBox(_objectShader, _scene.Scale, _scene.Camera.Curve, _scene.Camera.ReferencePointPosition);
    }

    public void RegisterCallbacks(Context context)
    {
        context.RegisterKeys(new List<Keys> { Keys.LeftShift, Keys.Space, Keys.W, Keys.S, Keys.A, Keys.D });
        context.RegisterUpdateFrameCallback((e) =>
        {
            // TODO commented out until we have context switching
            /*float steeringSum = 0;
            if (context.HeldKeys[Keys.A]) steeringSum += 1;
            if (context.HeldKeys[Keys.D]) steeringSum -= 1;
            float targetSpeedFraction = context.HeldKeys[Keys.W] ? 1f : context.HeldKeys[Keys.S] ? -1f : 0;*/
            Vector2 movementDirection = default;
            if (context.HeldKeys[Keys.W])
            {
                movementDirection = new Vector2(0, 1);
            }

            if (context.HeldKeys[Keys.S])
            {
                movementDirection += new Vector2(0, -1);
            }

            if (context.HeldKeys[Keys.A])
            {
                if (_scene.Player.CurrentSphereId == 0)
                    movementDirection += new Vector2(-1, 0);
                else
                    movementDirection -= new Vector2(-1, 0);
            }

            if (context.HeldKeys[Keys.D])
            {
                if (_scene.Player.CurrentSphereId == 0)
                    movementDirection += new Vector2(1, 0);
                else
                    movementDirection -= new Vector2(1, 0);
            }

            _scene.Player.UpdateCharacterGoals(_scene.SimulationManager.Simulation, _scene.Camera.Front, (float)e.Time,
                context.HeldKeys[Keys.Space], context.HeldKeys[Keys.LeftShift], movementDirection);

            if (_spherical)
            {
                int targetSphereId = 1 - _scene.Player.CurrentSphereId;
                if (_sphericalTransporter.TryTeleportTo(targetSphereId, _scene.Player, _scene.SimulationManager.Simulation, out var exitPoint))
                {
                    _sphericalTransporter.UpdateCamera(targetSphereId, _scene.Camera, exitPoint);
                    _objectShader.SetInt("characterSphere", targetSphereId);
                    _modelShader.SetInt("characterSphere", targetSphereId);
                    _rayMarkerShader.SetInt("characterSphere", targetSphereId);
                }
            }

            _scene.Camera.UpdateWithCharacter(_scene.Player);
        });

        context.RegisterKeyDownCallback(Keys.F3, () => _showBoundingBoxes = !_showBoundingBoxes);
    }

    public void Dispose()
    {
        _modelShader.Dispose();
        _objectShader.Dispose();
        _rayMarkerShader.Dispose();
    }
}