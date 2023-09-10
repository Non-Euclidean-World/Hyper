using BepuPhysics;
using BepuPhysics.CollisionDetection;
using BepuUtilities;
using BepuUtilities.Memory;
using Physics.RayCasting;

namespace Physics.Collisions;

/// <summary>
/// Utility class with objects and methods necessary to run the simulation
/// </summary>
/// <typeparam name="TNarrowPhaseCallbacks">Type handling the narrow phase handles</typeparam>
/// <typeparam name="TPoseIntegratorCallbacks">Type handling pose integration callbacks</typeparam>
public class SimulationManager<TNarrowPhaseCallbacks, TPoseIntegratorCallbacks> : IDisposable
    where TNarrowPhaseCallbacks : struct, INarrowPhaseCallbacks
    where TPoseIntegratorCallbacks : struct, IPoseIntegratorCallbacks
{
    public Simulation Simulation { get; private set; }
    public BufferPool BufferPool { get; private set; }
    public ThreadDispatcher ThreadDispatcher { get; private set; }

    private HitHandler _hitHandler;
    public readonly Buffer<RayHit> RayCastingResults;

    private bool _disposed = false;

    public SimulationManager(TNarrowPhaseCallbacks narrowPhaseCallbacks, TPoseIntegratorCallbacks poseIntegratorCallbacks, SolveDescription solveDescription, BufferPool bufferPool)

    {
        BufferPool = bufferPool;
        int targetThreadCount = int.Max(1, Environment.ProcessorCount > 4 ? Environment.ProcessorCount - 2 : Environment.ProcessorCount - 1);
        ThreadDispatcher = new ThreadDispatcher(targetThreadCount);
        Simulation = Simulation.Create(BufferPool, narrowPhaseCallbacks, poseIntegratorCallbacks, solveDescription);

        bufferPool.Take(1, out RayCastingResults);
        _hitHandler = new HitHandler { Hits = RayCastingResults };
    }

    public void Timestep(float dt)
    {
        Simulation.Timestep(dt, ThreadDispatcher);
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

    public void Dispose()
    {
        if (!_disposed)
        {
            _disposed = true;
            Simulation.Dispose();
            ThreadDispatcher.Dispose();
            BufferPool.Clear();
        }
    }
}
