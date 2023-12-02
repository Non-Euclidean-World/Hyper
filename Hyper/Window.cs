using Common;
using Hyper.Menu;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Hyper;

public class Window : GameWindow
{
    private readonly MainMenu _mainMenu;

    private Game? _game;
    
    private bool _isGameRunning;

    public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings, GeometryType geometryType)
        : base(gameWindowSettings, nativeWindowSettings)
    {
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        
        var windowHelper = new WindowHelper(this);
        _mainMenu = GetMainMenu(windowHelper, geometryType != GeometryType.None);
        if (geometryType == GeometryType.None)
            return;
        
        _game = new Game(nativeWindowSettings.Size.X, nativeWindowSettings.Size.Y, windowHelper, DefaultSaveName(), geometryType);
        CursorState = CursorState.Grabbed;
        _isGameRunning = true;
    }

    private MainMenu GetMainMenu(IWindowHelper windowHelper, bool isGameLoaded)
    {
        var mainMenu = new MainMenu(windowHelper, isGameLoaded);
        mainMenu.Resume += () =>
        {
            if (_game == null)
                return;
            CursorState = CursorState.Grabbed;
            _isGameRunning = true;
        };
        mainMenu.NewGame += (saveName, geometryType) =>
        {
            _isGameRunning = false;
            _game?.SaveAndClose();
            _game = new Game(Size.X, Size.Y, windowHelper, saveName, geometryType);
            CursorState = CursorState.Grabbed;
            _isGameRunning = true;
        };
        mainMenu.Delete += (saveName) =>
        {
            if (saveName == _game?.Settings.SaveName)
                return;
            SaveManager.DeleteSaves(new[] { saveName });
            mainMenu.Reload();
        };
        mainMenu.Load += (saveName) =>
        {
            _isGameRunning = false;
            _game?.SaveAndClose();

            _game = new Game(Size.X, Size.Y, windowHelper, saveName, GeometryType.Euclidean);
            CursorState = CursorState.Grabbed;
            _isGameRunning = true;
        };
        mainMenu.Quit += Close;

        return mainMenu;
    }

    public override void Close()
    {
        _isGameRunning = false;
        _game?.SaveAndClose();
        base.Close();
    }

    public void CloseNoSave()
    {
        base.Close();
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);

        if (_isGameRunning)
            _game?.RenderFrame(e);
        else
            _mainMenu.Render();

        SwapBuffers();
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        base.OnUpdateFrame(e);

        if (!IsFocused)
            return;
        if (_isGameRunning)
            _game?.UpdateFrame(e);
    }

    protected override void OnKeyDown(KeyboardKeyEventArgs e)
    {
        base.OnKeyDown(e);

        if (e.Key == Keys.Escape)
        {
            _isGameRunning = !_isGameRunning;
            CursorState = CursorState == CursorState.Grabbed ? CursorState.Normal : CursorState.Grabbed;
            return;
        }

        if (_isGameRunning)
        {
            _game?.KeyDown(e.Key);
            if (e.Key == Keys.T)
            {
                CursorState = CursorState.Normal;
                CursorState = CursorState.Grabbed;
            }
        }
        else
            _mainMenu.KeyDown(e);
    }

    protected override void OnKeyUp(KeyboardKeyEventArgs e)
    {
        base.OnKeyUp(e);

        if (_isGameRunning)
            _game?.KeyUp(e.Key);
    }

    protected override void OnMouseMove(MouseMoveEventArgs e)
    {
        base.OnMouseMove(e);

        if (_isGameRunning)
            _game?.MouseMove(e);
    }

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
        base.OnMouseDown(e);

        if (_isGameRunning)
            _game?.MouseDown(e.Button);
        else
            _mainMenu.Click();
    }

    protected override void OnMouseUp(MouseButtonEventArgs e)
    {
        base.OnMouseUp(e);

        if (_isGameRunning)
            _game?.MouseUp(e.Button);
    }

    protected override void OnMouseWheel(MouseWheelEventArgs e)
    {
        base.OnMouseWheel(e);

        if (_isGameRunning)
            _game?.MouseWheel(e);
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        base.OnResize(e);

        _game?.Resize(e);
    }

    protected override void OnFocusedChanged(FocusedChangedEventArgs e)
    {
        if (!e.IsFocused)
        {
            _isGameRunning = false;
            CursorState = CursorState.Normal;
        }
    }

    private static string DefaultSaveName()
        => DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss");
}