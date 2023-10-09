using Common.UserInput;
using Hyper.Controllers.Bots.Spawn;
using Hyper.Shaders.ModelShader;
using Hyper.Shaders.ObjectShader;
using Hyper.Transporters;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Hyper.Controllers.Bots;

internal class BotsController : IController, IInputSubscriber
{
    private readonly Scene _scene;

    private readonly AbstractModelShader _modelShader;

    private readonly AbstractObjectShader _objectShader;
    
    private readonly ITransporter _transporter;
    
    private readonly AbstractBotSpawnStrategy _spawnStrategy;

    private bool _showBoundingBoxes;

    public BotsController(Scene scene, 
        Context context, 
        AbstractModelShader modelShader, 
        AbstractObjectShader objectShader, 
        ITransporter transporter, 
        AbstractBotSpawnStrategy spawnStrategy)
    {
        _scene = scene;
        _modelShader = modelShader;
        _objectShader = objectShader;
        _transporter = transporter;
        _spawnStrategy = spawnStrategy;
        
        RegisterCallbacks(context);
    }

    public void Render()
    {
        _modelShader.SetUp(_scene.Camera, _scene.LightSources);
        foreach (var bot in _scene.Bots)
        {
            _modelShader.SetInt("sphere", bot.CurrentSphereId);
            bot.Render(_modelShader, _modelShader.GlobalScale, _scene.Camera.Curve, _scene.Camera.ReferencePointPosition);
        }

        if (!_showBoundingBoxes) return;
        _objectShader.SetUp(_scene.Camera, _scene.LightSources);
        foreach (var bot in _scene.Bots)
        {
            _modelShader.SetInt("sphere", bot.CurrentSphereId);
            bot.PhysicalCharacter.RenderBoundingBox(_objectShader, _objectShader.GlobalScale, _scene.Camera.Curve, _scene.Camera.ReferencePointPosition);
        }
    }
    
    public void RegisterCallbacks(Context context)
    {
        context.RegisterUpdateFrameCallback((e) =>
        {
            _spawnStrategy.Despawn();
            _spawnStrategy.Spawn();
            Move((float)e.Time);
        });
        
        context.RegisterKeyDownCallback(Keys.F3, () => _showBoundingBoxes = !_showBoundingBoxes);
    }

    private void Move(float time)
    {
        foreach (var bot in _scene.Bots)
        {
            bot.UpdateCharacterGoals(_scene.SimulationManager.Simulation, time);
            
            int targetSphereId = 1 - bot.CurrentSphereId;
            _transporter.TryTeleportTo(targetSphereId, bot, _scene.SimulationManager.Simulation, out _);
        }
    }

    public void Dispose()
    {
        _modelShader.Dispose();
    }
}