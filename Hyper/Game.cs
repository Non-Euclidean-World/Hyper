using System.Resources;
using Character.Shaders;
using Chunks.ChunkManagement;
using Chunks.MarchingCubes;
using Common;
using Common.UserInput;
using Hud.Shaders;
using Hyper.Controllers;
using Hyper.Shaders;
using NLog;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Hyper;

public class Game : IInputSubscriber
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public bool Loaded { get; private set; }
    
    private Scene _scene = null!;

    private IController[] _controllers = null!;

    private readonly Context _context = Context.Instance;

    private readonly Vector2i _size;

    public static string SavesLocation = Path.GetFullPath("saves");
    
    public Game(int width, int height)
    {
        _size = new Vector2i(width, height);
    }
    
    public void Close()
    {
        LogManager.Flush();
    }

    public void OnLoad(IWindowHelper windowHelper)
    {
        GL.ClearColor(0f, 0f, 0f, 1.0f);
        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        
        var seed = new Random().Next();
        Logger.Info($"Seed: {seed}");
        
        var scalarFieldGenerator = new ScalarFieldGenerator(seed);
        ChunkFactory chunkFactory = new ChunkFactory(scalarFieldGenerator);

        _scene = new Scene(_size.X / (float)_size.Y, 31);
        var objectShader = ObjectShader.Create();
        var modelShader = ModelShader.Create();
        var lightSourceShader = LightSourceShader.Create();
        var hudShader = HudShader.Create();

        _controllers = new IController[]
        {
            new PlayerController(_scene, modelShader, objectShader, lightSourceShader),
            new BotsController(_scene, modelShader, objectShader),
            new ChunksController(_scene, objectShader, chunkFactory),
            new ProjectilesController(_scene, objectShader),
            new VehiclesController(_scene, objectShader),
            new LightSourcesController(_scene, lightSourceShader),
            new HudController(windowHelper, hudShader),
        };
        
        Loaded = true;
    }

    public void OnRenderFrame(FrameEventArgs e)
    {
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        foreach (var controller in _controllers)
        {
            controller.Render();
        }
    }

    public void OnUpdateFrame(FrameEventArgs e)
    {
        foreach (var callback in _context.FrameUpdateCallbacks)
        {
            callback(e);
        }
        _context.ExecuteAllHeldCallbacks(InputType.Key, e);
        _context.ExecuteAllHeldCallbacks(InputType.MouseButton, e);
    }

    public void OnKeyDown(Keys key)
    {
        if (!_context.KeyDownCallbacks.ContainsKey(key))
            return;

        foreach (var callback in _context.KeyDownCallbacks[key])
        {
            callback();
        }
    }

    public void OnKeyUp(Keys key)
    {
        if (!_context.KeyUpCallbacks.ContainsKey(key))
            return;

        foreach (var callback in _context.KeyUpCallbacks[key])
        {
            callback();
        }
    }

    public void OnMouseMove(MouseMoveEventArgs e)
    {
        foreach (var callback in _context.MouseMoveCallbacks)
        {
            callback(e);
        }
    }

    public void OnMouseDown(MouseButton button)
    {
        if (!_context.ButtonDownCallbacks.ContainsKey(button))
            return;

        foreach (var callback in _context.ButtonDownCallbacks[button])
        {
            callback();
        }
    }

    public void OnMouseUp(MouseButton button)
    {
        if (!_context.ButtonUpCallbacks.ContainsKey(button))
            return;

        foreach (var callback in _context.ButtonUpCallbacks[button])
        {
            callback();
        }
    }

    public void OnMouseWheel(MouseWheelEventArgs e)
    {
        _scene.Camera.Fov -= e.OffsetY;
    }

    public void OnResize(ResizeEventArgs e)
    {
        GL.Viewport(0, 0, _size.X, _size.Y);
        _scene.Camera.AspectRatio = _size.X / (float)_size.Y;
    }

    public static void Save(string name)
    {
        var saveLocation = Path.Combine(SavesLocation, name);
        Directory.CreateDirectory(saveLocation);
        var chunkLocation = Path.Combine(saveLocation, "chunks");
        Directory.CreateDirectory(chunkLocation);
        foreach (var file in Directory.GetFiles(ChunkHandler.SaveLocation))
        {
            var fileName = Path.GetFileName(file);
            File.Copy(file, Path.Combine(chunkLocation, fileName));
        }
    }

    public static void Load(string name)
    {
        foreach (var file in Directory.GetFiles(ChunkHandler.SaveLocation))
        {
            File.Delete(file);
        }
        
        var saveLocation = Path.Combine(SavesLocation, name);
        var chunkLocation = Path.Combine(saveLocation, "chunks");
        foreach (var file in Directory.GetFiles(chunkLocation))
        {
            File.Copy(file, ChunkHandler.SaveLocation);
        }
    }

    public void RegisterCallbacks()
    {
        _context.RegisterKeys(new List<Keys> { Keys.Escape });

        _context.RegisterKeyDownCallback(Keys.Escape, Close);
    }
}