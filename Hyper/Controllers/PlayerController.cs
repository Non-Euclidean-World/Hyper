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
    
    private bool _showBoundingBoxes = false;

    public PlayerController(Scene scene, Context context, ModelShader modelShader, ObjectShader objectShader, LightSourceShader rayMarkerShader)
    {
        _scene = scene;
        _modelShader = modelShader;
        _objectShader = objectShader;
        _rayMarkerShader = rayMarkerShader;
        RegisterCallbacks(context);
    }

    public void Render()
    {
        _modelShader.SetUp(_scene.Camera, _scene.LightSources, _scene.Scale);
        _scene.Player.Render(_modelShader, _scene.Scale, _scene.Camera.ReferencePointPosition, _scene.Camera.FirstPerson);

        _rayMarkerShader.SetUp(_scene.Camera);
        _scene.Player.RenderRay(in _scene.SimulationManager.RayCastingResults[_scene.Player.RayId], _rayMarkerShader, _scene.Scale, _scene.Camera.ReferencePointPosition);

        if (!_showBoundingBoxes) return;
        _objectShader.SetUp(_scene.Camera, _scene.LightSources, _scene.Scale);
        _scene.Player.PhysicalCharacter.RenderBoundingBox(_objectShader, _scene.Scale, _scene.Camera.ReferencePointPosition);
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
                movementDirection += new Vector2(-1, 0);
            }

            if (context.HeldKeys[Keys.D])
            {
                movementDirection += new Vector2(1, 0);
            }

            _scene.Player.UpdateCharacterGoals(_scene.SimulationManager.Simulation, _scene.Camera.Front, (float)e.Time,
                context.HeldKeys[Keys.Space], context.HeldKeys[Keys.LeftShift], movementDirection);

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