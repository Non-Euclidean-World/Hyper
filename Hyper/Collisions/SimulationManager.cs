using BepuPhysics;
using BepuPhysics.CollisionDetection;
using BepuUtilities;
using BepuUtilities.Memory;

namespace Hyper.Collisions;
internal class SimulationManager<TNarrowPhaseCallbacks, TPoseIntegratorCallbacks> : IDisposable
    where TNarrowPhaseCallbacks : struct, INarrowPhaseCallbacks
    where TPoseIntegratorCallbacks : struct, IPoseIntegratorCallbacks
{
    public Simulation Simulation { get; private set; }
    public BufferPool BufferPool { get; private set; }
    public ThreadDispatcher ThreadDispatcher { get; private set; }

    private bool _disposed = false;

    public SimulationManager(TNarrowPhaseCallbacks narrowPhaseCallbacks, TPoseIntegratorCallbacks poseIntegratorCallbacks, SolveDescription solveDescription, BufferPool bufferPool)

    {
        BufferPool = bufferPool;
        int targetThreadCount = int.Max(1, Environment.ProcessorCount > 4 ? Environment.ProcessorCount - 2 : Environment.ProcessorCount - 1);
        ThreadDispatcher = new ThreadDispatcher(targetThreadCount);
        Simulation = Simulation.Create(BufferPool, narrowPhaseCallbacks, poseIntegratorCallbacks, solveDescription);
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
