using Common.UserInput;
using Hyper.Shaders.ModelShader;
using Hyper.Shaders.ObjectShader;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Hyper.Controllers;

internal class BotsController : IController, IInputSubscriber
{
    private readonly Scene _scene;

    private readonly AbstractModelShader _modelShader;

    private readonly AbstractObjectShader _objectShader;

    private float _elapsedSeconds = 0;

    private bool _showBoundingBoxes = false;

    public BotsController(Scene scene, Context context, AbstractModelShader modelShader, AbstractObjectShader objectShader)
    {
        _scene = scene;
        _modelShader = modelShader;
        _objectShader = objectShader;
        RegisterCallbacks(context);
    }

    public void Render()
    {
        _modelShader.SetUp(_scene.Camera, _scene.LightSources, sphere: 0); // TODO different spheres
        foreach (var bot in _scene.Bots)
        {
            bot.Render(_modelShader, _modelShader.GlobalScale, _scene.Camera.Curve, _scene.Camera.ReferencePointPosition);
        }

        if (!_showBoundingBoxes) return;
        _objectShader.SetUp(_scene.Camera, _scene.LightSources, sphere: 0); // TODO current sphere
        foreach (var bot in _scene.Bots)
        {
            bot.PhysicalCharacter.RenderBoundingBox(_objectShader, _objectShader.GlobalScale, _scene.Camera.Curve, _scene.Camera.ReferencePointPosition);
        }
    }

    public void RegisterCallbacks(Context context)
    {
        context.RegisterUpdateFrameCallback((e) =>
        {
            foreach (var bot in _scene.Bots)
            {
                _elapsedSeconds += (float)e.Time;
                Vector3 movement = new Vector3(MathF.Sin(_elapsedSeconds / 3), 0, MathF.Cos(_elapsedSeconds / 3)); // these poor fellas are cursed with eternal running in circles
                bot.UpdateCharacterGoals(_scene.SimulationManager.Simulation, movement, (float)e.Time,
                    tryJump: false, sprint: false, movementDirection: Vector2.UnitY);
            }
        });

        context.RegisterKeyDownCallback(Keys.F3, () => _showBoundingBoxes = !_showBoundingBoxes);
    }

    public void Dispose()
    {
        _modelShader.Dispose();
    }
}