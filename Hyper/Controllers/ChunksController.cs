using Chunks.ChunkManagement;
using Common.UserInput;
using Hyper.Shaders;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Hyper.Controllers;

internal class ChunksController : IController, IInputSubscriber
{
    private readonly Scene _scene;

    private readonly ObjectShader _shader;

    private readonly ChunkWorker _chunkWorker;

    private float _buildTime = 0;

    private float _mineTime = 0;

    public ChunksController(Scene scene, Context context, ObjectShader shader, ChunkFactory chunkFactory, ChunkHandler chunkHandler)
    {
        _scene = scene;
        _shader = shader;
        _chunkWorker = new ChunkWorker(_scene.Chunks, _scene.SimulationManager, chunkFactory, chunkHandler);
        RegisterCallbacks(context);
    }

    public void Render()
    {
        _shader.SetUp(_scene.Camera, _scene.LightSources, _scene.Scale);

        foreach (var chunk in _scene.Chunks)
        {
            chunk.Render(_shader, _scene.Scale, _scene.Camera.ReferencePointPosition);
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
                if (!chunk.IsInside(location)) continue;
                if (!_chunkWorker.IsOnUpdateQueue(chunk))
                {
                    chunk.Mine(
                        _scene.Player.GetRayEndpoint(
                            in _scene.SimulationManager.RayCastingResults[_scene.Player.RayId]), (float)e.Time + _mineTime);
                    _chunkWorker.EnqueueUpdatingChunk(chunk);
                    _mineTime = 0;
                }
                else _mineTime += (float)e.Time;
                return;
            }
        });

        context.RegisterMouseButtonHeldCallback(MouseButton.Right, (e) =>
        {
            foreach (var chunk in _scene.Chunks)
            {
                var location =
                    _scene.Player.GetRayEndpoint(in _scene.SimulationManager.RayCastingResults[_scene.Player.RayId]);
                if (!chunk.IsInside(location)) continue;
                if (!_chunkWorker.IsOnUpdateQueue(chunk))
                {
                    chunk.Build(
                        _scene.Player.GetRayEndpoint(
                            in _scene.SimulationManager.RayCastingResults[_scene.Player.RayId]), (float)e.Time + _buildTime);
                    _chunkWorker.EnqueueUpdatingChunk(chunk);
                    _buildTime = 0;
                }
                else _buildTime += (float)e.Time;
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