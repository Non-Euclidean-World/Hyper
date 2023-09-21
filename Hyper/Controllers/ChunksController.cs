using Chunks.ChunkManagement;
using Common;
using Common.UserInput;
using Hyper.Shaders;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Hyper.Controllers;

internal class ChunksController : IController, IInputSubscriber
{
    private readonly Scene _scene;

    private readonly ObjectShader _shader;

    private readonly ChunkWorker _chunkWorker;

    public ChunksController(Scene scene, ObjectShader shader, Settings settings)
    {
        _scene = scene;
        _shader = shader;
        _chunkWorker = new ChunkWorker(_scene.Chunks, _scene.SimulationManager, settings);
        RegisterCallbacks();
    }

    public void Render()
    {
        _shader.SetUp(_scene.Camera, _scene.LightSources, _scene.Scale);

        foreach (var chunk in _scene.Chunks)
        {
            chunk.Render(_shader, _scene.Scale, _scene.Camera.ReferencePointPosition);
        }
    }

    public void RegisterCallbacks()
    {
        var context = Context.Instance;

        context.RegisterMouseButtons(new List<MouseButton> { MouseButton.Left, MouseButton.Right });
        context.RegisterMouseButtonHeldCallback(MouseButton.Left, (e) =>
        {
            foreach (var chunk in _scene.Chunks)
            {
                if (chunk.Mine(_scene.Player.GetRayEndpoint(in _scene.SimulationManager.RayCastingResults[_scene.Player.RayId]), 3, (float)e.Time))
                {
                    _chunkWorker.EnqueueUpdatingChunk(chunk);
                    return;
                }
            }
        });

        context.RegisterMouseButtonHeldCallback(MouseButton.Right, (e) =>
        {
            foreach (var chunk in _scene.Chunks)
            {
                if (chunk.Build(_scene.Player.GetRayEndpoint(in _scene.SimulationManager.RayCastingResults[_scene.Player.RayId]), 3, (float)e.Time))
                {
                    _chunkWorker.EnqueueUpdatingChunk(chunk);
                    return;
                }
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