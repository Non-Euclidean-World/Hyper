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

public class Game
{
    public bool IsRunning = true;

    private readonly Scene _scene;

    private readonly IController[] _controllers;

    private readonly Context _context = new();

    private Vector2i _size;

    private readonly Settings _settings;

    public Game(int width, int height, IWindowHelper windowHelper, string saveName)
    {
        _size = new Vector2i(width, height);

        GL.ClearColor(0f, 0f, 0f, 1.0f);
        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

        _settings = Settings.Load(saveName);
        _settings.AspectRatio = (float)width / height;
        
        var scalarFieldGenerator = new ScalarFieldGenerator(_settings.Seed);
        var chunkFactory = new ChunkFactory(scalarFieldGenerator);
        var chunkHandler = new ChunkHandler(_settings.SaveName);
        
        _scene = new Scene(_size.X / (float)_size.Y, scalarFieldGenerator.AvgElevation, _context);
        var objectShader = ObjectShader.Create();
        var modelShader = ModelShader.Create();
        var lightSourceShader = LightSourceShader.Create();
        var hudShader = HudShader.Create();

        _controllers = new IController[]
        {
            new PlayerController(_scene, _context, modelShader, objectShader, lightSourceShader),
            new BotsController(_scene, _context, modelShader, objectShader, _settings),
            new ChunksController(_scene, _context, objectShader, chunkFactory, chunkHandler, _settings),
            new ProjectilesController(_scene, _context, objectShader),
            new VehiclesController(_scene, _context, objectShader),
            new LightSourcesController(_scene, lightSourceShader),
            new HudController(_scene, _context, windowHelper, hudShader),
        };
    }

    public void SaveAndClose()
    {
        foreach (var controller in _controllers)
        {
            controller.Dispose();
        }
        _scene.Dispose(); // Scene dispose needs to be after controller dispose.
        _settings.Save();
        LogManager.Flush();
        IsRunning = false;
    }

    public void RenderFrame(FrameEventArgs e)
    {
        if (!IsRunning) return;
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        foreach (var controller in _controllers)
        {
            controller.Render();
        }
    }

    public void UpdateFrame(FrameEventArgs e)
    {
        if (e.Time == 0) return;
        foreach (var callback in _context.FrameUpdateCallbacks)
        {
            callback(e);
        }
        _context.ExecuteAllHeldCallbacks(InputType.Key, e);
        _context.ExecuteAllHeldCallbacks(InputType.MouseButton, e);
    }

    public void KeyDown(Keys key)
    {
        if (!_context.KeyDownCallbacks.ContainsKey(key))
            return;

        foreach (var callback in _context.KeyDownCallbacks[key])
        {
            callback();
        }
    }

    public void KeyUp(Keys key)
    {
        if (!_context.KeyUpCallbacks.ContainsKey(key))
            return;

        foreach (var callback in _context.KeyUpCallbacks[key])
        {
            callback();
        }
    }

    public void MouseMove(MouseMoveEventArgs e)
    {
        foreach (var callback in _context.MouseMoveCallbacks)
        {
            callback(e);
        }
    }

    public void MouseDown(MouseButton button)
    {
        if (!_context.ButtonDownCallbacks.ContainsKey(button))
            return;

        foreach (var callback in _context.ButtonDownCallbacks[button])
        {
            callback();
        }
    }

    public void MouseUp(MouseButton button)
    {
        if (!_context.ButtonUpCallbacks.ContainsKey(button))
            return;

        foreach (var callback in _context.ButtonUpCallbacks[button])
        {
            callback();
        }
    }

    public void MouseWheel(MouseWheelEventArgs e)
    {
        _scene.Camera.Fov -= e.OffsetY;
    }

    public void Resize(ResizeEventArgs e)
    {
        GL.Viewport(0, 0, e.Width, e.Height);
        _scene.Camera.AspectRatio = e.Width / (float)e.Height;
        _settings.AspectRatio = e.Width / (float)e.Height;
        _size = e.Size;
    }
}