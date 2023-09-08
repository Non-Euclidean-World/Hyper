using Character.Shaders;
using Common;
using Common.UserInput;
using Hyper.Shaders;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Hyper.Controllers;

internal class PlayerController : IController, IInputSubscriber
{
    private readonly Scene _scene;

    private readonly ModelShader _shader;

    private readonly Shader _objectShader;

    public PlayerController(Scene scene, ModelShader shader, Shader objectShader)
    {
        _scene = scene;
        _shader = shader;
        _objectShader = objectShader;
        RegisterCallbacks();
    }

    public void Render()
    {
        _shader.SetUp(_scene.Camera, _scene.LightSources, _scene.Scale);

#if BOUNDING_BOXES
        _scene.Player.PhysicalCharacter.RenderBoundingBox(_objectShader, _scene.Scale, _scene.Camera.ReferencePointPosition);
#endif
        _scene.Player.Render(_shader, _scene.Scale, _scene.Camera.ReferencePointPosition, _scene.Camera.FirstPerson);
    }

    public void RegisterCallbacks()
    {
        var context = Context.Instance;

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
    }
}