using Hyper.Meshes;
using Hyper.UserInput;
using NLog;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Hyper;

internal class Window : GameWindow, IInputSubscriber
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    private CancellationTokenSource _debugCancellationTokenSource = null!;

    private Scene _scene = null!;

    private readonly UserInput.Context _context = UserInput.Context.Instance;

    public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
        : base(gameWindowSettings, nativeWindowSettings)
    {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        StartDebugThreadAsync();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

        RegisterCallbacks();
    }

    public override void Close()
    {
        StopDebugThread();
        base.Close();
        LogManager.Flush();
    }

    protected override void OnLoad()
    {
        base.OnLoad();

        GL.ClearColor(0f, 0f, 0f, 1.0f);
        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

        _scene = new Scene(Size.X / (float)Size.Y);

        CursorState = CursorState.Grabbed;
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        _scene.Render();

        SwapBuffers();
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        base.OnUpdateFrame(e);

        if (!IsFocused)
        {
            return;
        }

        _context.ExecuteAllHeldCallbacks(InputType.Key, e);
        _context.ExecuteAllHeldCallbacks(InputType.MouseButton, e);
    }

    protected override void OnKeyDown(KeyboardKeyEventArgs e)
    {
        base.OnKeyDown(e);

        if (!_context.KeyDownCallbacks.ContainsKey(e.Key))
            return;

        foreach (var callback in _context.KeyDownCallbacks[e.Key])
        {
            callback();
        }
    }

    protected override void OnKeyUp(KeyboardKeyEventArgs e)
    {
        base.OnKeyUp(e);

        if (!_context.KeyUpCallbacks.ContainsKey(e.Key))
            return;

        foreach (var callback in _context.KeyUpCallbacks[e.Key])
        {
            callback();
        }
    }

    protected override void OnMouseMove(MouseMoveEventArgs e)
    {
        base.OnMouseMove(e);

        foreach (var callback in _context.MouseMoveCallbacks)
        {
            callback(e);
        }
    }

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
        base.OnMouseDown(e);

        if (e.Button == MouseButton.Middle)
        {
            var projectile = new Projectile(CubeMesh.Vertices, _scene.Camera.ReferencePointPosition + 1 / Scene.Scale * Vector3.UnitY, _scene.Camera.Front, 20f, 5f);
            _scene.Projectiles.Add(projectile);
        }

        if (!_context.ButtonDownCallbacks.ContainsKey(e.Button))
            return;

        foreach (var callback in _context.ButtonDownCallbacks[e.Button])
        {
            callback();
        }
    }

    protected override void OnMouseUp(MouseButtonEventArgs e)
    {
        base.OnMouseUp(e);

        if (!_context.ButtonUpCallbacks.ContainsKey(e.Button))
            return;

        foreach (var callback in _context.ButtonUpCallbacks[e.Button])
        {
            callback();
        }
    }

    protected override void OnMouseWheel(MouseWheelEventArgs e)
    {
        base.OnMouseWheel(e);

        _scene.Camera.Fov -= e.OffsetY;
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);

        GL.Viewport(0, 0, Size.X, Size.Y);
        _scene.Camera.AspectRatio = Size.X / (float)Size.Y;
        _scene.Hud.AspectRatio = Size.X / (float)Size.Y;
    }

    private async Task StartDebugThreadAsync()
    {
        _debugCancellationTokenSource = new CancellationTokenSource();
        await Task.Run(() => Command(_debugCancellationTokenSource.Token));
    }

    private void StopDebugThread()
    {
        _debugCancellationTokenSource.Cancel();
    }

    private void Command(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            string? command = Console.ReadLine();
            if (command == null)
                return;

            Logger.Info($"[Command]{command}");

            try
            {
                var args = command.Split(' ');
                var key = args[0];
                args = args.Skip(1).ToArray();

                switch (key)
                {
                    case "camera":
                        _scene.Camera.Command(args);
                        break;
                    case "hud":
                        _scene.Hud.Command(args);
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                Console.WriteLine(ex.Message);
            }
        }
    }

    public void RegisterCallbacks()
    {
        _context.RegisterKeys(new List<Keys> { Keys.Escape });

        _context.RegisterKeyDownCallback(Keys.Escape, Close);
    }
}