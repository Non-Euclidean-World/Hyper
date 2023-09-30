using Character.GameEntities;
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

    private const int MaxBots = 10;

    private const int BotsMinSpawnRadius = 10;
    
    private const int BotsMaxSpawnRadius = 20;
    
    private const int BotsDespawnRadius = 30;
    
    private bool _showBoundingBoxes = false;

    public BotsController(Scene scene, Context context, ModelShader shader, ObjectShader objectShader)
    {
        _scene = scene;
        _shader = shader;
        _objectShader = objectShader;
        
        Spawn();
        RegisterCallbacks(context);
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
    
    public void RegisterCallbacks(Context context)
    {
        context.RegisterUpdateFrameCallback((e) =>
        {
            Despawn();
            Spawn();
            Move((float)e.Time);
        });
        
        context.RegisterKeyDownCallback(Keys.F3, () => _showBoundingBoxes = !_showBoundingBoxes);
    }

    private void Move(float time)
    {
        foreach (var bot in _scene.Bots)
        {
            _elapsedSeconds += time;
            Vector3 movement = new Vector3(MathF.Sin(_elapsedSeconds / 3), 0, MathF.Cos(_elapsedSeconds / 3)); // these poor fellas are cursed with eternal running in circles
            bot.UpdateCharacterGoals(_scene.SimulationManager.Simulation, movement, time,
                tryJump: false, sprint: false, movementDirection: Vector2.UnitY);
        }
    }

    private void Despawn()
    {
        _scene.Bots.RemoveAll(bot =>
        {
            var distance = (bot.PhysicalCharacter.Pose.Position - _scene.Player.PhysicalCharacter.Pose.Position);

            if (!(Math.Abs(distance.X) > BotsDespawnRadius || Math.Abs(distance.Z) > BotsDespawnRadius)) return false;
            Console.WriteLine($"Despawning bot {bot.BodyHandle}");
            bot.Dispose();
            _scene.SimulationMembers.Remove(bot.BodyHandle);
            return true;
        });
    }

    private void Spawn()
    {
        for (int i = 0; i < MaxBots - _scene.Bots.Count; i++)
        {
            var rand = new Random();
            var x = rand.Next(0, 2) == 0 ? rand.Next(-BotsMaxSpawnRadius, -BotsMinSpawnRadius) : rand.Next(BotsMinSpawnRadius, BotsMaxSpawnRadius);
            var z = rand.Next(0, 2) == 0 ? rand.Next(-BotsMaxSpawnRadius, -BotsMinSpawnRadius) : rand.Next(BotsMinSpawnRadius, BotsMaxSpawnRadius);
            var position = new Vector3(x + _scene.Player.PhysicalCharacter.Pose.Position.X, 31, z + _scene.Player.PhysicalCharacter.Pose.Position.Z); // TODO remove the magic number
            var bot = new Humanoid(_scene.CreatePhysicalHumanoid(position));
            Console.WriteLine($"Spawning bot {bot.BodyHandle}");
            _scene.SimulationMembers.Add(bot.BodyHandle, bot);
            _scene.SimulationManager.RegisterContactCallback(bot.BodyHandle, contactInfo => bot.ContactCallback(contactInfo, _scene.SimulationMembers));
            _scene.Bots.Add(bot);
        }
    }

    public void Dispose()
    {
        _shader.Dispose();
    }
}