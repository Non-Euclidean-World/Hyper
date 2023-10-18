﻿using BepuPhysics;
using BepuPhysics.Collidables;
using BepuUtilities.Memory;
using Physics.Collisions;

namespace Character.Projectiles;
public class Projectile : ISimulationMember
{
    public BodyHandle BodyHandle { get; private set; }

    public bool IsDead { get; private set; }

    public ProjectileMesh Mesh { get; private set; }

    public int CurrentSphereId { get; set; }

    public IList<BodyHandle> BodyHandles { get; private set; } = null!;

    private TypedIndex _shape;

    private float _lifeTime;

    private Projectile(ProjectileMesh mesh, float lifeTime, int currentSphereId)
    {
        Mesh = mesh;
        _lifeTime = lifeTime;
        CurrentSphereId = currentSphereId;
    }

    /// <summary>
    /// Creates a lightweight projectile.
    /// </summary>
    /// <param name="simulation"></param>
    /// <param name="properties"></param>
    /// <param name="initialPose"></param>
    /// <param name="initialVelocity"></param>
    /// <param name="mesh"></param>
    /// <param name="lifeTime">Lifetime threshold in seconds</param>
    /// <returns></returns>
    public static Projectile CreateStandardProjectile(Simulation simulation, CollidableProperty<SimulationProperties> properties,
        in RigidPose initialPose, in BodyVelocity initialVelocity, ProjectileMesh mesh, float lifeTime, int currentSphereId = 0)
    {
        var projectileShape = new Box(mesh.Size.X, mesh.Size.Y, mesh.Size.Z);

        var projectile = new Projectile(mesh, lifeTime, currentSphereId);
        projectile._shape = simulation.Shapes.Add(projectileShape);
        var inertia = projectileShape.ComputeInertia(0.01f);

        RigidPose adjustedInitialPose;
        var initialPosition = initialPose.Position;
        if (currentSphereId == 1)
        {
            adjustedInitialPose = new RigidPose(new System.Numerics.Vector3(initialPosition.X, initialPosition.Y, initialPosition.Z), initialPose.Orientation);
        }
        else
        {
            adjustedInitialPose = initialPose;
        }
        projectile.BodyHandle = simulation.Bodies.Add(BodyDescription.CreateDynamic(adjustedInitialPose, initialVelocity, inertia, new CollidableDescription(projectile._shape, 0.5f), 0.01f));
        projectile.BodyHandles = new BodyHandle[1] { projectile.BodyHandle };
        ref var bodyProperties = ref properties.Allocate(projectile.BodyHandle);
        bodyProperties = new SimulationProperties { Friction = 2f, Filter = new SubgroupCollisionFilter(projectile.BodyHandle.Value, 0) };

        return projectile;
    }

    /// <summary>
    /// Updates the projectile's position.
    /// Removes the projectile from the simulation once it's been existing longer than the lifetime threshold.
    /// </summary>
    /// <param name="simulation"></param>
    /// <param name="dt"></param>
    /// <param name="pool"></param>
    public void Update(Simulation simulation, float dt, BufferPool pool)
    {
        var body = new BodyReference(BodyHandle, simulation.Bodies);
        Mesh.Update(body.Pose);

        _lifeTime -= dt;
        if (_lifeTime < 0) IsDead = true;
    }

    public void Dispose(Simulation simulation, BufferPool bufferPool)
    {
        simulation.Bodies.Remove(BodyHandle);
        simulation.Shapes.RemoveAndDispose(_shape, bufferPool);
        Mesh.Dispose();
    }
}
