using Character.Shaders;
using Common.UserInput;
using Hud;
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
    public bool Loaded { get; private set; } = false;
    
    private CancellationTokenSource _debugCancellationTokenSource = null!;

    private Scene _scene = null!;

    private IController[] _controllers = null!;

    private readonly Context _context = Context.Instance;

    public Vector2i Size;
    
    public Game(int width, int height)
    {
        Size = new Vector2i(width, height);
    }
    
    public void Close()
    {
        LogManager.Flush();
    }

    public void OnLoad()
    {
        GL.ClearColor(0f, 0f, 0f, 1.0f);
        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

        _scene = new Scene(Size.X / (float)Size.Y);
        var objectShader = ObjectShader.Create();
        var modelShader = ModelShader.Create();
        var lightSourceShader = LightSourceShader.Create();
        var hudShader = HudShader.Create();

        var hudHelper = new HudHelper();

        _controllers = new IController[]
        {
            new PlayerController(_scene, modelShader, objectShader, lightSourceShader),
            new BotsController(_scene, modelShader, objectShader),
            new ChunksController(_scene, objectShader),
            new ProjectilesController(_scene, objectShader),
            new VehiclesController(_scene, objectShader),
            new LightSourcesController(_scene, lightSourceShader),
            new HudController(hudHelper, hudShader),
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
        GL.Viewport(0, 0, Size.X, Size.Y);
        _scene.Camera.AspectRatio = Size.X / (float)Size.Y;
    }

    public void RegisterCallbacks()
    {
        _context.RegisterKeys(new List<Keys> { Keys.Escape });

        _context.RegisterKeyDownCallback(Keys.Escape, Close);
    }
}