using Character.Shaders;
using Common.UserInput;
using Hyper.Shaders;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Hyper.Controllers;

internal class BotsController : IController, IInputSubscriber
{
    private readonly Scene _scene;

    private readonly ModelShader _shader;

    private readonly ObjectShader _objectShader;
    
    private float _elapsedSeconds = 0;
    
    private bool _showBoundingBoxes = false;

    public BotsController(Scene scene, ModelShader shader, ObjectShader objectShader)
    {
        _scene = scene;
        _shader = shader;
        _objectShader = objectShader;
        RegisterCallbacks();
    }

    public void Render()
    {
        _shader.SetUp(_scene.Camera, _scene.LightSources, _scene.Scale);
        foreach (var bot in _scene.Bots)
        {
            bot.Render(_shader, _scene.Scale, _scene.Camera.ReferencePointPosition);
        }
        
        if (!_showBoundingBoxes) return;
        _objectShader.SetUp(_scene.Camera, _scene.LightSources, _scene.Scale);
        foreach (var bot in _scene.Bots)
        {
            bot.PhysicalCharacter.RenderBoundingBox(_objectShader, _scene.Scale, _scene.Camera.ReferencePointPosition);
        }
    }

    public void RegisterCallbacks()
    {
        var context = Context.Instance;

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
        _shader.Dispose();
    }
}