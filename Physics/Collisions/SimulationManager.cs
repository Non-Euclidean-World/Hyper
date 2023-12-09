using BepuPhysics;
using BepuUtilities;
using BepuUtilities.Memory;
using Physics.ContactCallbacks;
using Physics.RayCasting;

using ContactCallback = System.Action<Physics.ContactCallbacks.ContactInfo>;

namespace Physics.Collisions;

/// <summary>
/// Utility class with objects and methods necessary to run the simulation
/// </summary>
/// <typeparam name="TPoseIntegratorCallbacks">Type handling pose integration callbacks</typeparam>
public class SimulationManager<TPoseIntegratorCallbacks> : IDisposable
    where TPoseIntegratorCallbacks : struct, IPoseIntegratorCallbacks
{
    /// <summary>
    /// Gets the simulation instance managed by this manager.
    /// </summary>
    public Simulation Simulation { get; private init; }

    /// <summary>
    /// Gets the buffer pool used by the simulation.
    /// </summary>
    public BufferPool BufferPool { get; private init; }

    /// <summary>
    /// Gets the character controllers associated with the simulation.
    /// </summary>
    public CharacterControllers CharacterControllers { get; private init; }

    /// <summary>
    /// Gets the properties associated with the simulation.
    /// </summary>
    public CollidableProperty<SimulationProperties> Properties { get; private init; }

    /// <summary>
    /// Gets the dictionary of contact callbacks associated with body handles.
    /// </summary>
    public Dictionary<BodyHandle, ContactCallback> ContactCallbacks { get; private init; }

    /// <summary>
    /// Gets the buffer storing raycasting results.
    /// </summary>
    public readonly Buffer<RayHit> RayCastingResults;

    private readonly ContactEventHandler _contactEventHandler;

    private readonly ThreadDispatcher _threadDispatcher;

    private readonly ContactEvents _contactEvents;

    private HitHandler _hitHandler;

    private bool _disposed = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="SimulationManager{TPoseIntegratorCallbacks}"/> class.
    /// </summary>
    /// <param name="poseIntegratorCallbacks">Callbacks for pose integration.</param>
    /// <param name="solveDescription">Description of the solver to use.</param>
    public SimulationManager(TPoseIntegratorCallbacks poseIntegratorCallbacks, SolveDescription solveDescription)

    {
        BufferPool = new BufferPool();
        CharacterControllers = new CharacterControllers(BufferPool);
        Properties = new CollidableProperty<SimulationProperties>();

        int targetThreadCount = int.Max(1, Environment.ProcessorCount > 4 ? Environment.ProcessorCount - 2 : Environment.ProcessorCount - 1);
        _threadDispatcher = new ThreadDispatcher(targetThreadCount);

        _contactEvents = new ContactEvents(_threadDispatcher, BufferPool);
        NarrowPhaseCallbacks narrowPhaseCallbacks = new NarrowPhaseCallbacks(CharacterControllers, Properties, _contactEvents);
        Simulation = Simulation.Create(BufferPool, narrowPhaseCallbacks, poseIntegratorCallbacks, solveDescription);

        ContactCallbacks = new Dictionary<BodyHandle, ContactCallback>();
        _contactEventHandler = new ContactEventHandler(Simulation, ContactCallbacks);

        BufferPool.Take(1, out RayCastingResults);
        _hitHandler = new HitHandler { Hits = RayCastingResults };
    }

    /// <summary>
    /// Advances the simulation by a specified time increment.
    /// </summary>
    /// <param name="dt">Time increment for the simulation step.</param>
    public void Timestep(float dt)
    {
        Simulation.Timestep(dt, _threadDispatcher);
    }

    /// <summary>
    /// Flushes the contact events in the simulation.
    /// </summary>
    public void FlushContactEvents()
    {
        _contactEvents.Flush();
    }

    /// <summary>
    /// Resets the raycasting result for a specific ray.
    /// </summary>
    /// <param name="rayCaster">The ray caster instance.</param>
    /// <param name="rayId">Identifier for the ray to reset.</param>
    public void ResetRayCastingResult(IRayCaster rayCaster, int rayId)
    {
        RayCastingResults[rayId].T = rayCaster.RayMaximumT;
        RayCastingResults[rayId].Hit = false;
    }

    /// <summary>
    /// Performs a raycast in the simulation using the provided ray caster and identifier.
    /// </summary>
    /// <param name="rayCaster">The ray caster instance.</param>
    /// <param name="rayId">Identifier for the ray to cast.</param>
    public void RayCast(IRayCaster rayCaster, int rayId)
    {
        Simulation.RayCast(rayCaster.RayOrigin, rayCaster.RayDirection, rayCaster.RayMaximumT, ref _hitHandler, rayId);
    }

    /// <summary>
    /// Registers a contact callback for a specific body handle in the simulation.
    /// </summary>
    /// <param name="bodyHandle">The handle of the body to register the callback for.</param>
    /// <param name="callback">The contact callback to register.</param>
    /// <returns>True if the callback was successfully registered; otherwise, false.</returns>
    public bool RegisterContactCallback(BodyHandle bodyHandle, ContactCallback callback)
    {
        if (!ContactCallbacks.TryAdd(bodyHandle, callback))
            return false;

        _contactEvents.Register(bodyHandle, _contactEventHandler);
        return true;
    }

    /// <summary>
    /// Unregisters a contact callback associated with a specific body handle in the simulation.
    /// </summary>
    /// <param name="bodyHandle">The handle of the body to unregister the callback for.</param>
    /// <returns>True if the callback was successfully unregistered; otherwise, false.</returns>
    public bool UnregisterContactCallback(BodyHandle bodyHandle)
    {
        if (!ContactCallbacks.Remove(bodyHandle))
            return false;

        _contactEvents.Unregister(bodyHandle);
        return true;
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _disposed = true;
            Simulation.Dispose();
            _threadDispatcher.Dispose();
            BufferPool.Clear();
        }
    }
}
