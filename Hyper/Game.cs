using Chunks.MarchingCubes;
using Common;
using Common.UserInput;
using Hyper.Controllers;
using Hyper.Controllers.Factories;
using Hyper.PlayerData;
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

    private readonly float _globalScale = 0.05f;

    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public Game(int width, int height, IWindowHelper windowHelper, string saveName, GeometryType geometryType) // TODO this is definitely getting out of hand
    {
        _size = new Vector2i(width, height);

        GL.ClearColor(0f, 0f, 0f, 1.0f);
        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

        if (!Settings.SaveExists(saveName))
        {
            Random rand = new Random();
            _settings = new Settings(rand.Next(), saveName, (float)width / height, geometryType);
        }
        else
        {
            _settings = Settings.Load(saveName); // TODO it's confusing as hell but geometryType variable is INVALID from this point onward
            _settings.AspectRatio = (float)width / height;
        }
        Logger.Info($"Seed: {_settings.Seed}");

        float curve = _settings.GeometryType switch
        {
            GeometryType.Euclidean => 0f,
            GeometryType.Hyperbolic => -1f,
            GeometryType.Spherical => 1f,
            _ => throw new NotImplementedException(),
        };

        var scalarFieldGenerator = new ScalarFieldGenerator(_settings.Seed);
        var camera = new Camera(aspectRatio: _size.X / (float)_size.Y, curve, near: 0.01f, far: 200f, _globalScale, _context)
        {
            ReferencePointPosition = (5f + scalarFieldGenerator.AvgElevation) * Vector3.UnitY
        };
        _scene = new Scene(camera, _settings.GeometryType == GeometryType.Spherical ? 0 : scalarFieldGenerator.AvgElevation, _context, windowHelper);
        IControllerFactory controllerFactory = _settings.GeometryType switch
        {
            GeometryType.Spherical => new SphericalControllerFactory(_scene, _context, windowHelper, scalarFieldGenerator, _globalScale),
            GeometryType.Hyperbolic or GeometryType.Euclidean => new StandardControllerFactory(_scene, _context, windowHelper, scalarFieldGenerator, _globalScale),
            _ => throw new NotImplementedException(),
        };

        _controllers = controllerFactory.CreateControllers(_settings);
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