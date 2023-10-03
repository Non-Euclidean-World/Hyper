using Common;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Hyper;

public class Window : GameWindow
{
    private Game _game;

    private CommandInterpreter _interpreter;

    public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
        : base(gameWindowSettings, nativeWindowSettings)
    {
        _game = new Game(nativeWindowSettings.Size.X, nativeWindowSettings.Size.Y, new WindowHelper(this), DefaultSaveName(), GeometryType.Euclidean);
        _interpreter = new CommandInterpreter(this);
        CursorState = CursorState.Grabbed;
    }

    public override void Close()
    {
        if (_game.IsRunning)
            _game.SaveAndClose();
        base.Close();
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);

        if (!_game.IsRunning)
            return;

        _game.RenderFrame(e);
        SwapBuffers();
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        base.OnUpdateFrame(e);

        if (!IsFocused)
            return;
        if (_game.IsRunning)
            _game.UpdateFrame(e);
    }

    protected override void OnKeyDown(KeyboardKeyEventArgs e)
    {
        base.OnKeyDown(e);

        if (_game.IsRunning)
            _game.KeyDown(e.Key);

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

        if (_game.IsRunning)
            _game.KeyUp(e.Key);
    }

    protected override void OnMouseMove(MouseMoveEventArgs e)
    {
        base.OnMouseMove(e);

        if (_game.IsRunning)
            _game.MouseMove(e);
    }

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
        base.OnMouseDown(e);

        if (_game.IsRunning)
            _game.MouseDown(e.Button);
    }

    protected override void OnMouseUp(MouseButtonEventArgs e)
    {
        base.OnMouseUp(e);

        if (_game.IsRunning)
            _game.MouseUp(e.Button);
    }

    protected override void OnMouseWheel(MouseWheelEventArgs e)
    {
        base.OnMouseWheel(e);

        if (_game.IsRunning)
            _game.MouseWheel(e);
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);

        _game.Resize(e);
    }

    protected override void OnFocusedChanged(FocusedChangedEventArgs e)
    {
        if (e.IsFocused)
            _game.IsRunning = true;
        else
            _game.IsRunning = false;
    }

    private void Command()
    {
        while (true)
        {
            string? line = Console.ReadLine();
            if (line is null)
                return;
            try
            {
                _interpreter.ParseLine(line, ref _game, out bool exitCli);
                Console.WriteLine();
                if (exitCli)
                    return;
            }
            catch (Exception e)
            {
                Console.WriteLine($"[ERROR] {e.Message}");
            }
        }
    }

    private static string DefaultSaveName()
        => DateTime.UtcNow.ToString("dd-MM-yyyy_HH-mm-ss");
}