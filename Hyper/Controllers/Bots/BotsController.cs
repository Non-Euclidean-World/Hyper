using Common.UserInput;
using Hyper.Controllers.Bots.Spawn;
using Hyper.Shaders.ModelShader;
using Hyper.Transporters;

namespace Hyper.Controllers.Bots;

internal class BotsController : IController, IInputSubscriber
{
    private readonly Scene _scene;

    private readonly AbstractModelShader _modelShader;

    private readonly ITransporter _transporter;

    private readonly AbstractBotSpawnStrategy _spawnStrategy;

    public BotsController(Scene scene,
        Context context,
        AbstractModelShader modelShader,
        ITransporter transporter,
        AbstractBotSpawnStrategy spawnStrategy)
    {
        _scene = scene;
        _modelShader = modelShader;
        _transporter = transporter;
        _spawnStrategy = spawnStrategy;

        RegisterCallbacks(context);
    }

    public void Render()
    {
        _modelShader.SetUp(_scene.Camera, _scene.LightSources, _scene.FlashLights, shininess: 32, _scene.GlobalScale);
        foreach (var bot in _scene.Bots)
        {
            _modelShader.SetInt("sphere", bot.CurrentSphereId);
            bot.Render(_modelShader, _scene.GlobalScale, _scene.Camera.Curve, _scene.Camera.ReferencePointPosition);
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
    }

    private void Move(float time)
    {
        foreach (var bot in _scene.Bots)
        {
            bot.UpdateCharacterGoals(_scene.SimulationManager.Simulation, time, _scene.Player.PhysicalCharacter);

            int targetSphereId = 1 - bot.CurrentSphereId;
            _transporter.TryTeleportTo(targetSphereId, bot, _scene.SimulationManager.Simulation, out _);
        }
    }

    public void Dispose()
    {
        _modelShader.Dispose();
    }
}