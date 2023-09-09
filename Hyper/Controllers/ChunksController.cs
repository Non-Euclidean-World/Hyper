using Chunks;
using Common.UserInput;
using Hyper.Shaders;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Physics.TypingUtils;

namespace Hyper.Controllers;

internal class ChunksController : IController, IInputSubscriber
{
    private readonly Scene _scene;

    private readonly ObjectShader _shader;
    
    private readonly ChunkWorker _chunkWorker;
    
    public ChunksController(Scene scene, ObjectShader shader, int seed)
    {
        _scene = scene;
        _shader = shader;
        _chunkWorker = new ChunkWorker(_scene.Chunks, _scene.SimulationManager, seed);
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
                if (chunk.Mine(Conversions.ToOpenTKVector(_scene.Player.GetCharacterRay(_scene.Camera.Front, 1)), 3, (float)e.Time))
                {
                    chunk.UpdateCollisionSurface(_scene.SimulationManager.Simulation, _scene.SimulationManager.BufferPool);
                    return;
                }
            }
        });

        context.RegisterMouseButtonHeldCallback(MouseButton.Right, (e) =>
        {
            foreach (var chunk in _scene.Chunks)
            {
                if (chunk.Build(Conversions.ToOpenTKVector(_scene.Player.GetCharacterRay(_scene.Camera.Front, 3)), 3, (float)e.Time))
                {
                    chunk.UpdateCollisionSurface(_scene.SimulationManager.Simulation, _scene.SimulationManager.BufferPool);
                    return;
                }
            }
        });
        
        context.RegisterUpdateFrameCallback(_ => _chunkWorker.Update(_scene.Camera.ReferencePointPosition));
    }
}