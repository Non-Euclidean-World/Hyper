using Chunks.ChunkManagement.ChunkWorkers;
using Common.UserInput;
using Hyper.Shaders;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Hyper.Controllers;

internal class ChunksController : IController, IInputSubscriber
{
    private readonly Scene _scene;

    private readonly AbstractObjectShader _shader;

    private readonly IChunkWorker _chunkWorker;

    private float _buildTime = 0;

    private float _mineTime = 0;

    public ChunksController(Scene scene, Context context, AbstractObjectShader shader, IChunkWorker chunkWorker)
    {
        _scene = scene;
        _shader = shader;
        _chunkWorker = chunkWorker;
        RegisterCallbacks(context);
    }

    public void Render()
    {
        foreach (var chunk in _scene.Chunks)
        {
            _shader.SetUp(_scene.Camera, _scene.LightSources, chunk.Sphere);
            chunk.Render(_shader, _shader.GlobalScale, _scene.Camera.Curve, _scene.Camera.ReferencePointPosition);
        }
    }

    public void RegisterCallbacks(Context context)
    {
        context.RegisterMouseButtons(new List<MouseButton> { MouseButton.Left, MouseButton.Right });
        context.RegisterMouseButtonHeldCallback(MouseButton.Left, (e) =>
        {
            foreach (var chunk in _scene.Chunks)
            {
                var location =
                    _scene.Player.GetRayEndpoint(in _scene.SimulationManager.RayCastingResults[_scene.Player.RayId]);
                if (!chunk.IsInside(location))
                    continue;
                if (!_chunkWorker.IsOnUpdateQueue(chunk))
                {
                    chunk.Mine(location, (float)e.Time + _mineTime);
                    _chunkWorker.EnqueueUpdatingChunk(chunk);
                    _mineTime = 0;
                }
                else
                    _mineTime += (float)e.Time;
                return;
            }
        });

        context.RegisterMouseButtonHeldCallback(MouseButton.Right, (e) =>
        {
            foreach (var chunk in _scene.Chunks)
            {
                var location =
                    _scene.Player.GetRayEndpoint(in _scene.SimulationManager.RayCastingResults[_scene.Player.RayId]);
                if (!chunk.IsInside(location))
                    continue;
                if (!_chunkWorker.IsOnUpdateQueue(chunk))
                {
                    chunk.Build(location, (float)e.Time + _buildTime);
                    _chunkWorker.EnqueueUpdatingChunk(chunk);
                    _buildTime = 0;
                }
                else
                    _buildTime += (float)e.Time;
                return;
            }
        });

        context.RegisterUpdateFrameCallback(_ => _chunkWorker.Update(_scene.Camera.ReferencePointPosition));
    }

    public void Dispose()
    {
        _shader.Dispose();
        _chunkWorker.Dispose();
    }
}