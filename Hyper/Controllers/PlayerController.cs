using System.Text.Json;
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

    private readonly ModelShader _modelShader;

    private readonly ObjectShader _objectShader;

    private readonly LightSourceShader _rayMarkerShader;
    
    private string SaveFile => Path.Combine(_settings.CurrentSaveLocation, "player.json");

    private readonly Settings _settings = Settings.Instance;

    public PlayerController(Scene scene, ModelShader modelShader, ObjectShader objectShader, LightSourceShader rayMarkerShader)
    {
        _scene = scene;
        _modelShader = modelShader;
        _objectShader = objectShader;
        _rayMarkerShader = rayMarkerShader;
        RegisterCallbacks();
    }

    public void Render()
    {
        _modelShader.SetUp(_scene.Camera, _scene.LightSources, _scene.Scale);
        _scene.Player.Render(_modelShader, _scene.Scale, _scene.Camera.ReferencePointPosition, _scene.Camera.FirstPerson);

        _rayMarkerShader.SetUp(_scene.Camera);
        _scene.Player.RenderRay(in _scene.SimulationManager.RayCastingResults[_scene.Player.RayId], _rayMarkerShader, _scene.Scale, _scene.Camera.ReferencePointPosition);

#if BOUNDING_BOXES
        _objectShader.SetUp(_scene.Camera, _scene.LightSources, _scene.Scale);
        _scene.Player.PhysicalCharacter.RenderBoundingBox(_objectShader, _scene.Scale, _scene.Camera.ReferencePointPosition);
#endif
    }

    private void Save()
    {
        var settings = new Dictionary<string, string>
        {
            {"position", SerializationHelper.Serialize(_scene.Player.PhysicalCharacter.Pose.Position)},
        };

        File.WriteAllText(SaveFile, JsonSerializer.Serialize(settings));
        
        _scene.Player.Dispose(_scene.SimulationManager.Simulation);
    }

    private void Start()
    {
        Vector3 characterInitialPosition;
        if (File.Exists(SaveFile))
        {
            var json = File.ReadAllText(SaveFile);
            var playerData = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

            characterInitialPosition = SerializationHelper.Deserialize(playerData["position"]);
        }
        else characterInitialPosition = new Vector3(0, 31, 15);
        
        _scene.Player = new Player.Player(_scene.CreatePhysicalHumanoid(characterInitialPosition));
        _scene.SimulationMembers.Add(_scene.Player.BodyHandle, _scene.Player);
        _scene.SimulationManager.RegisterContactCallback(_scene.Player.BodyHandle, contactInfo => _scene.Player.ContactCallback(contactInfo, _scene.SimulationMembers));
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
        
        context.RegisterCloseCallback(Save);
        context.RegisterStartCallback(Start);
    }
}