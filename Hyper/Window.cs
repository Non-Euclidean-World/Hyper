using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Hyper;

public class Window : GameWindow
{
    private readonly Game _game;

    public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
        : base(gameWindowSettings, nativeWindowSettings)
    {
        _game = new Game(nativeWindowSettings.Size.X, nativeWindowSettings.Size.Y);
    }

    public override void Close()
    {
        base.Close();
        _game.Close();
    }

    protected override void OnLoad()
    {
        base.OnLoad();
        _game.OnLoad();
        CursorState = CursorState.Grabbed;
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);

        _game.OnRenderFrame(e);

        SwapBuffers();
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        base.OnUpdateFrame(e);

        if (!IsFocused)
        {
            return;
        }

        _game.OnUpdateFrame(e);
    }

    protected override void OnKeyDown(KeyboardKeyEventArgs e)
    {
        base.OnKeyDown(e);

        _game.OnKeyDown(e.Key);
        
        if (e.Key == Keys.Escape)
        {
            Close();
        }
    }

    protected override void OnKeyUp(KeyboardKeyEventArgs e)
    {
        base.OnKeyUp(e);

        _game.OnKeyUp(e.Key);
    }

    protected override void OnMouseMove(MouseMoveEventArgs e)
    {
        base.OnMouseMove(e);

        _game.OnMouseMove(e);
    }

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
        base.OnMouseDown(e);

        _game.OnMouseDown(e.Button);
    }

    protected override void OnMouseUp(MouseButtonEventArgs e)
    {
        base.OnMouseUp(e);

        _game.OnMouseUp(e.Button);
    }

    protected override void OnMouseWheel(MouseWheelEventArgs e)
    {
        base.OnMouseWheel(e);

        _game.OnMouseWheel(e);
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);

        _game.OnResize(e);
    }
}