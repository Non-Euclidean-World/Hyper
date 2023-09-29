using Chunks.ChunkManagement;
using Common.UserInput;
using Hyper.Shaders;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Hyper.Controllers;

internal class ChunksController : IController, IInputSubscriber
{
    private readonly Scene _scene;

    private readonly ObjectShader _shader;

    public ChunksController(Scene scene, Context context, ObjectShader shader)
    {
        _scene = scene;
        _shader = shader;
        RegisterCallbacks(context);
    }

    public void Render()
    {
        _shader.SetUp(_scene.Camera, _scene.LightSources, _scene.Scale);

        foreach (var chunk in _scene.ChunkWorker.Chunks)
        {
            chunk.Render(_shader, _scene.Scale, _scene.Camera.ReferencePointPosition);
        }
    }

    public void RegisterCallbacks(Context context)
    {
        context.RegisterMouseButtons(new List<MouseButton> { MouseButton.Left, MouseButton.Right });
        context.RegisterUpdateFrameCallback(_ => _scene.ChunkWorker.Update(_scene.Camera.ReferencePointPosition));
    }

    public void Dispose()
    {
        _shader.Dispose();
    }
}