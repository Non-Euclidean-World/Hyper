using BepuPhysics;
using BepuPhysics.CollisionDetection;
using BepuUtilities;
using BepuUtilities.Memory;

namespace Hyper.Collisions;
internal class SimulationManager<TNarrowPhaseCallbacks, TPoseIntegratorCallbacks> : IDisposable
    where TNarrowPhaseCallbacks : struct, INarrowPhaseCallbacks
    where TPoseIntegratorCallbacks : struct, IPoseIntegratorCallbacks
{
    public Simulation Simulation { get => _simulation; }
    public BufferPool BufferPool { get => _bufferPool; }
    public ThreadDispatcher ThreadDispatcher { get => _threadDispatcher; }

    private bool _disposed = false;
    private Simulation _simulation;
    private BufferPool _bufferPool;
    private ThreadDispatcher _threadDispatcher;

    public SimulationManager(TNarrowPhaseCallbacks narrowPhaseCallbacks, TPoseIntegratorCallbacks poseIntegratorCallbacks, SolveDescription solveDescription)

    {
        _bufferPool = new BufferPool();
        int targetThreadCount = int.Max(1, Environment.ProcessorCount > 4 ? Environment.ProcessorCount - 2 : Environment.ProcessorCount - 1);
        _threadDispatcher = new ThreadDispatcher(targetThreadCount);
        _simulation = Simulation.Create(_bufferPool, narrowPhaseCallbacks, poseIntegratorCallbacks, solveDescription);
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _disposed = true;
            _simulation.Dispose();
            _threadDispatcher.Dispose();
            _bufferPool.Clear();
        }
    }
}
