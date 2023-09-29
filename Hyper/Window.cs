using Common;
using NLog;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Hyper;

public class Window : GameWindow
{
    private Game _game;

    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
        : base(gameWindowSettings, nativeWindowSettings)
    {
        _game = new Game(nativeWindowSettings.Size.X, nativeWindowSettings.Size.Y, new WindowHelper(this), DateTime.UtcNow.ToString("dd-MM-yyyy_HH-mm-ss"));
        CursorState = CursorState.Grabbed;
    }

    public override void Close()
    {
        if (_game.IsRunning) _game.SaveAndClose();
        base.Close();
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);

        if (!_game.IsRunning) return;
        
        _game.RenderFrame(e);
        SwapBuffers();
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        base.OnUpdateFrame(e);

        if (!IsFocused) return;
        if (_game.IsRunning) _game.UpdateFrame(e);
    }

    protected override void OnKeyDown(KeyboardKeyEventArgs e)
    {
        base.OnKeyDown(e);

        if (_game.IsRunning) _game.KeyDown(e.Key);

        if (e.Key == Keys.Escape)
        {
            Close();
        }

        if (e.Key == Keys.T)
        {
            CursorState = CursorState.Normal;
            Command();
            CursorState = CursorState.Grabbed;
        }
    }

    protected override void OnKeyUp(KeyboardKeyEventArgs e)
    {
        base.OnKeyUp(e);

        if (_game.IsRunning) _game.KeyUp(e.Key);
    }

    protected override void OnMouseMove(MouseMoveEventArgs e)
    {
        base.OnMouseMove(e);

        if (_game.IsRunning) _game.MouseMove(e);
    }

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
        base.OnMouseDown(e);

        if (_game.IsRunning) _game.MouseDown(e.Button);
    }

    protected override void OnMouseUp(MouseButtonEventArgs e)
    {
        base.OnMouseUp(e);

        if (_game.IsRunning) _game.MouseUp(e.Button);
    }

    protected override void OnMouseWheel(MouseWheelEventArgs e)
    {
        base.OnMouseWheel(e);

        if (_game.IsRunning) _game.MouseWheel(e);
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);
        
        _game.Resize(e);
    }

    protected override void OnFocusedChanged(FocusedChangedEventArgs e)
    {
        if (e.IsFocused) _game.IsRunning = true;
        else _game.IsRunning = false;
    }

    private void Command()
    {
        while (true)
        {
            string? command = Console.ReadLine();
            if (command is null)
                return;

            try
            {
                Logger.Info($"[Command]{command}");
                var args = command.Split(' ');

                switch (args[0])
                {
                    case "resume":
                        return;
                    case "exit":
                        Close();
                        return;
                    case "load":
                        if (!SaveManager.GetSaves().Contains(args[1]))
                            throw new FileNotFoundException("This save does not exist.");
                        if (_game.IsRunning) _game.SaveAndClose();
                        _game = new Game(Size.X, Size.Y, new WindowHelper(this), args[1]);
                        CursorState = CursorState.Grabbed;
                        return;
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
            catch (Exception ex)
            {
                Logger.Error($"[Command]Error: {ex.Message}");
            }
        }
    }
}