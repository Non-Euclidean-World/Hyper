using BepuPhysics;
using BepuPhysics.Collidables;
using BepuUtilities.Memory;
using Physics.Collisions;

namespace Character.Projectiles;
/// <summary>
/// A projectile that can be fired by a character.
/// </summary>
public class Projectile : ISimulationMember
{
    /// <summary>
    /// Whether or not the projectile is dead and should be removed from the scene.
    /// </summary>
    public bool IsDead { get; private set; }
    /// <summary>
    /// The mesh of the projectile.
    /// </summary>
    public ProjectileMesh Mesh { get; }
    /// <summary>
    /// The id of the sphere the projectile is currently in.
    /// </summary>
    public int CurrentSphereId { get; set; }
    /// <summary>
    /// A list of all the body handles that make up the projectile.
    /// </summary>
    public IList<BodyHandle> BodyHandles { get; private set; } = null!;
    
    private BodyHandle _bodyHandle;

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
    /// <param name="simulation">The physics simulation.</param>
    /// <param name="properties">The properties of the simulation.</param>
    /// <param name="initialPose">The initial pose of the projectile.</param>
    /// <param name="initialVelocity">The initial velocity of the projectile.</param>
    /// <param name="mesh">The mesh of the projectile.</param>
    /// <param name="lifeTime">Lifetime threshold in seconds</param>
    /// <param name="currentSphereId">The id of the sphere the projectile is created in.</param>
    /// <returns>An instance of the <see cref="Projectile"/> class.</returns>
    public static Projectile CreateStandardProjectile(
        Simulation simulation, 
        CollidableProperty<SimulationProperties> properties,
        in RigidPose initialPose, 
        in BodyVelocity initialVelocity, 
        ProjectileMesh mesh, 
        float lifeTime, 
        int currentSphereId = 0)
    {
        var projectileShape = new Box(mesh.Size.X, mesh.Size.Y, mesh.Size.Z);

        var projectile = new Projectile(mesh, lifeTime, currentSphereId)
        {
            _shape = simulation.Shapes.Add(projectileShape)
        };
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
        projectile._bodyHandle = simulation.Bodies.Add(BodyDescription.CreateDynamic(adjustedInitialPose, initialVelocity, inertia, new CollidableDescription(projectile._shape, 0.5f), 0.01f));
        projectile.BodyHandles = new BodyHandle[1] { projectile._bodyHandle };
        ref var bodyProperties = ref properties.Allocate(projectile._bodyHandle);
        bodyProperties = new SimulationProperties { Friction = 2f, Filter = new SubgroupCollisionFilter(projectile._bodyHandle.Value, 0) };

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
        var body = new BodyReference(_bodyHandle, simulation.Bodies);
        Mesh.Update(body.Pose);

        _lifeTime -= dt;
        if (_lifeTime < 0) IsDead = true;
    }

    /// <summary>
    /// Disposes of the projectile.
    /// </summary>
    /// <param name="simulation">The physics simulation.</param>
    /// <param name="bufferPool">The pool of buffers used by the simulation.</param>
    public void Dispose(Simulation simulation, BufferPool bufferPool)
    {
        simulation.Bodies.Remove(_bodyHandle);
        simulation.Shapes.RemoveAndDispose(_shape, bufferPool);
        Mesh.Dispose();
    }
}
