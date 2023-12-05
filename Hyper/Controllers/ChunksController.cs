using Chunks.ChunkManagement.ChunkWorkers;
using Common.UserInput;
using Hyper.Shaders.ObjectShader;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Hyper.Controllers;

internal class ChunksController : IController, IInputSubscriber
{
    private readonly Scene _scene;

    private readonly AbstractObjectShader _shader;

    private readonly IChunkWorker _chunkWorker;

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
            _shader.SetUp(_scene.Camera, _scene.LightSources, _scene.FlashLights, shininess: 1f, _scene.GlobalScale, chunk.Sphere);
            chunk.Render(_shader, _scene.GlobalScale, _scene.Camera.Curve, _scene.Camera.ReferencePointPosition);
        }
    }

    public void RegisterCallbacks(Context context)
    {
        context.RegisterMouseButtons(new List<MouseButton> { MouseButton.Left, MouseButton.Right });
        context.RegisterUpdateFrameCallback(_ => _chunkWorker.Update(_scene.Camera.ReferencePointPosition));
    }

    public void Dispose()
    {
        _shader.Dispose();
        _chunkWorker.Dispose();
    }
}