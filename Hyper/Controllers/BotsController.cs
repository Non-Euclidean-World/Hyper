﻿using Character.Shaders;
using Common.UserInput;
using Hyper.Shaders;
using OpenTK.Mathematics;

namespace Hyper.Controllers;

internal class BotsController : IController, IInputSubscriber
{
    private readonly Scene _scene;

    private readonly ModelShader _shader;

    private readonly ObjectShader _objectShader;

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
#if BOUNDING_BOXES
        _objectShader.SetUp(_scene.Camera, _scene.LightSources, _scene.Scale);
        foreach (var bot in _scene.Bots)
        {
            bot.PhysicalCharacter.RenderBoundingBox(_objectShader, _scene.Scale, _scene.Camera.ReferencePointPosition);
        }
#endif
    }

    public void RegisterCallbacks()
    {
        var context = Context.Instance;

        context.RegisterUpdateFrameCallback((e) =>
        {
            foreach (var bot in _scene.Bots)
            {
                float tMs = _scene.Stopwatch.ElapsedMilliseconds;
                Vector3 movement = new Vector3(MathF.Sin(tMs / 3000), 0, MathF.Cos(tMs / 3000)); // these poor fellas are cursed with eternal running in circles
                bot.UpdateCharacterGoals(_scene.SimulationManager.Simulation, movement, (float)e.Time,
                    tryJump: false, sprint: false, movementDirection: Vector2.UnitY);
            }
        });
    }
    
    public void Dispose()
    {
        _shader.Dispose();
    }
}