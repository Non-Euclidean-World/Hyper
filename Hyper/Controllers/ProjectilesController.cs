using BepuPhysics;
using Character.Projectiles;
using Common.UserInput;
using Hyper.Shaders.ObjectShader;
using Hyper.Transporters;
using Physics.TypingUtils;

namespace Hyper.Controllers;

internal class ProjectilesController : IController, IInputSubscriber
{
    private readonly Scene _scene;

    private readonly AbstractObjectShader _shader;

    private readonly ITransporter _transporter;

    public ProjectilesController(Scene scene, Context context, AbstractObjectShader shader, ITransporter transporter)
    {
        _scene = scene;
        _shader = shader;
        _transporter = transporter;
        RegisterCallbacks(context);
    }

    private void UpdateProjectiles(float dt)
    {
        _scene.Projectiles.RemoveAll(x => x.IsDead);
        foreach (var projectile in _scene.Projectiles)
        {
            projectile.Update(_scene.SimulationManager.Simulation, dt, _scene.SimulationManager.BufferPool);

            int targetSphereId = 1 - projectile.CurrentSphereId;
            _transporter.TryTeleportTo(targetSphereId, projectile, _scene.SimulationManager.Simulation, out _);

            if (projectile.IsDead)
            {
                projectile.Dispose(_scene.SimulationManager.Simulation, _scene.SimulationManager.BufferPool);
                _scene.SimulationMembers.Remove(projectile.BodyHandle);
            }
        }
    }

    private void CreateProjectile()
    {
        var q = Helpers.CreateQuaternionFromTwoVectors(System.Numerics.Vector3.UnitX, Conversions.ToNumericsVector(_scene.Camera.Front));
        var projectile = Projectile.CreateStandardProjectile(_scene.SimulationManager.Simulation,
            _scene.SimulationManager.Properties,
            new RigidPose(_scene.Player.RayOrigin, q),
            Conversions.ToNumericsVector(_scene.Camera.Front) * 15,
            new ProjectileMesh(2, 0.5f, 0.5f), lifeTime: 5, _scene.Player.CurrentSphereId); // let's throw some refrigerators
        _scene.Projectiles.Add(projectile);
        _scene.SimulationMembers.Add(projectile.BodyHandle, projectile);
    }

    public void Render()
    {
        foreach (var projectile in _scene.Projectiles)
        {
            if (projectile.IsDead)
                continue;
            _shader.SetUp(_scene.Camera, _scene.LightSources, projectile.CurrentSphereId);
            projectile.Mesh.Render(_shader, _shader.GlobalScale, _scene.Camera.Curve, _scene.Camera.ReferencePointPosition);
        }
    }

    public void RegisterCallbacks(Context context)
    {
        context.RegisterUpdateFrameCallback((e) => UpdateProjectiles((float)e.Time));
    }

    public void Dispose()
    {
        _shader.Dispose();
    }
}