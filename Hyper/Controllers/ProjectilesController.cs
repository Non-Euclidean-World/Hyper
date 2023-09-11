using BepuPhysics;
using Character.Projectiles;
using Common.UserInput;
using Hyper.Shaders;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Physics.TypingUtils;

namespace Hyper.Controllers;

internal class ProjectilesController : IController, IInputSubscriber
{
    private readonly Scene _scene;

    private readonly ObjectShader _shader;

    public ProjectilesController(Scene scene, ObjectShader shader)
    {
        _scene = scene;
        _shader = shader;
        RegisterCallbacks();
    }

    private void UpdateProjectiles(float dt)
    {
        _scene.Projectiles.RemoveAll(x => x.IsDead);
        foreach (var projectile in _scene.Projectiles)
        {
            projectile.Update(_scene.SimulationManager.Simulation, dt, _scene.SimulationManager.BufferPool);
        }
    }

    private void CreateProjectile()
    {
        var q = Helpers.CreateQuaternionFromTwoVectors(System.Numerics.Vector3.UnitX, Conversions.ToNumericsVector(_scene.Camera.Front));
        var projectile = Projectile.CreateStandardProjectile(_scene.SimulationManager.Simulation,
            _scene.SimulationManager.Properties,
            new RigidPose(_scene.Player.RayOrigin, q),
            Conversions.ToNumericsVector(_scene.Camera.Front) * 15,
            new ProjectileMesh(2, 0.5f, 0.5f), lifeTime: 5); // let's throw some refrigerators
        _scene.Projectiles.Add(projectile);
    }

    public void Render()
    {
        _shader.SetUp(_scene.Camera, _scene.LightSources, _scene.Scale);

        foreach (var projectile in _scene.Projectiles)
        {
            projectile.Mesh.Render(_shader, _scene.Scale, _scene.Camera.ReferencePointPosition);
        }
    }

    public void RegisterCallbacks()
    {
        var context = Context.Instance;

        context.RegisterUpdateFrameCallback((e) => UpdateProjectiles((float)e.Time));
        context.RegisterKeyDownCallback(Keys.P, CreateProjectile);
    }
}