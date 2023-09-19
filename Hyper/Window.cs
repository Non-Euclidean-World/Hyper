using Common;
using NLog;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Hyper;

public class Window : GameWindow
{
    private readonly Game _game;

    private readonly Thread _console;

    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
        : base(gameWindowSettings, nativeWindowSettings)
    {
        _game = new Game(nativeWindowSettings.Size.X, nativeWindowSettings.Size.Y);
        _game.Start(Guid.NewGuid().ToString(), new WindowHelper(this));
        CursorState = CursorState.Grabbed;
        _console = new Thread(Command)
        {
            IsBackground = true
        };
    }

    public override void Close()
    {
        base.Close();
        if (_game.IsRunning) _game.Close();
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);

        if (_game.IsRunning) _game.OnRenderFrame(e);

        SwapBuffers();
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        base.OnUpdateFrame(e);

        if (!IsFocused)
        {
            return;
        }

        if (_game.IsRunning) _game.OnUpdateFrame(e);
    }

    protected override void OnKeyDown(KeyboardKeyEventArgs e)
    {
        base.OnKeyDown(e);

        if (_game.IsRunning) _game.OnKeyDown(e.Key);

        if (e.Key == Keys.Escape)
        {
            Close();
        }

        if (e.Key == Keys.T)
        {
            if (_console.IsAlive) StopDebugThread();
            else StartDebugThreadAsync();
        }
    }

    protected override void OnKeyUp(KeyboardKeyEventArgs e)
    {
        base.OnKeyUp(e);

        if (_game.IsRunning) _game.OnKeyUp(e.Key);
    }

    protected override void OnMouseMove(MouseMoveEventArgs e)
    {
        base.OnMouseMove(e);

        if (_game.IsRunning) _game.OnMouseMove(e);
    }

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
        base.OnMouseDown(e);

        if (_game.IsRunning) _game.OnMouseDown(e.Button);
    }

    protected override void OnMouseUp(MouseButtonEventArgs e)
    {
        base.OnMouseUp(e);

        if (_game.IsRunning) _game.OnMouseUp(e.Button);
    }

    protected override void OnMouseWheel(MouseWheelEventArgs e)
    {
        base.OnMouseWheel(e);

        if (_game.IsRunning) _game.OnMouseWheel(e);
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);

        if (_game.IsRunning) _game.OnResize(e);
    }

    private void StartDebugThreadAsync()
    {
        _console.Start();
    }

    private void StopDebugThread()
    {
        _console.Interrupt();
    }

    private void Command()
    {
        try
        {
            while (true)
            {
                string? command = Console.ReadLine();
                if (command is null)
                    return;

                Logger.Info($"[Command]{command}");
                var args = command.Split(' ');

                switch (args[0])
                {
                    case "exit":
                        Close();
                        return;
                    case "start":
                        _game.Start(args[1], new WindowHelper(this));
                        CursorState = CursorState.Grabbed;
                        break;
                    case "save":
                        _game.Close();
                        break;
                    case "delete":
                        SaveManager.DeleteSave(args[1]);
                        break;
                    case "show":
                        if (args[1] == "saves")
                        {
                            foreach (var file in SaveManager.GetSaves())
                            {
                                Console.WriteLine(file);
                            }
                        }
                        break;
                }
            }
        }
        catch (ThreadInterruptedException ex)
        {
            Logger.Info($"[Command]Thread interrupted: {ex.Message}");
        }
    }
}