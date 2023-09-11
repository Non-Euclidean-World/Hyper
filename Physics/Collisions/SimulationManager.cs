using BepuPhysics;
using BepuUtilities;
using BepuUtilities.Memory;
using Physics.Collisions.Bepu;
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
    public Simulation Simulation { get; private init; }

    public BufferPool BufferPool { get; private init; }

    public CharacterControllers CharacterControllers { get; private init; }

    public CollidableProperty<SimulationProperties> Properties { get; private init; }

    public Dictionary<BodyHandle, ContactCallback> ContactCallbacks { get; private init; }

    public readonly Buffer<RayHit> RayCastingResults;
    private readonly ContactEventHandler _contactEventHandler;
    private readonly ThreadDispatcher _threadDispatcher;
    private readonly ContactEvents _contactEvents;
    private HitHandler _hitHandler;

    private bool _disposed = false;

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

    public void Timestep(float dt)
    {
        Simulation.Timestep(dt, _threadDispatcher);
    }

    public void FlushContactEvents()
    {
        _contactEvents.Flush();
    }

    public void ResetRayCastingResult(IRayCaster rayCaster, int rayId)
    {
        RayCastingResults[rayId].T = rayCaster.RayMaximumT;
        RayCastingResults[rayId].Hit = false;
    }

    public void RayCast(IRayCaster rayCaster, int rayId)
    {
        Simulation.RayCast(rayCaster.RayOrigin, rayCaster.RayDirection, rayCaster.RayMaximumT, ref _hitHandler, rayId);
    }

    public bool RegisterContactCallback(BodyHandle bodyHandle, ContactCallback callback)
    {
        if (!ContactCallbacks.TryAdd(bodyHandle, callback))
            return false;

        _contactEvents.Register(bodyHandle, _contactEventHandler);
        return true;
    }

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
