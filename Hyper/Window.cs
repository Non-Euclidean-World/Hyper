using BepuPhysics.Collidables;
using Common;
using Hud.Shaders;
using Hyper.Menu;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Hyper;

public class Window : GameWindow
{
    private readonly MainMenu _mainMenu;
    
    private Game _game;

    private readonly CommandInterpreter _interpreter;

    public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings, GeometryType geometryType)
        : base(gameWindowSettings, nativeWindowSettings)
    {
        var windowHelper = new WindowHelper(this);
        _game = new Game(nativeWindowSettings.Size.X, nativeWindowSettings.Size.Y, windowHelper, DefaultSaveName(), geometryType);
        _interpreter = new CommandInterpreter(this);
        _mainMenu = GetMainMenu(windowHelper);
        CursorState = CursorState.Grabbed;
    }

    private MainMenu GetMainMenu(IWindowHelper windowHelper)
    {
        var mainMenu = new MainMenu(windowHelper);
        mainMenu.Resume += () =>
        {
            CursorState = CursorState.Grabbed;
            _game.IsRunning = true;
        };
        mainMenu.NewGame += (saveName, geometryType) =>
        {
            _game.SaveAndClose();
            _game = new Game(Size.X, Size.Y, windowHelper, saveName, geometryType);
            CursorState = CursorState.Grabbed;
        };
        mainMenu.Delete += (saveName) =>
        {
            if (saveName == _game.Settings.SaveName)
                return;
            SaveManager.DeleteSaves(new []{saveName});
            mainMenu.Reload();
        };
        mainMenu.Load += (saveName) =>
        {
            _game.SaveAndClose();

            _game = new Game(Size.X, Size.Y, windowHelper, saveName, GeometryType.Euclidean);
            CursorState = CursorState.Grabbed;
        };
        mainMenu.Quit += Close;

        return mainMenu;
    }

    public override void Close()
    {
        _game.SaveAndClose();
        base.Close();
    }

    public void CloseNoSave()
    {
        base.Close();
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);

        if (_game.IsRunning)
            _game.RenderFrame(e);
        else
            _mainMenu.Render();
        
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
        
        if (e.Key == Keys.Escape)
        {
            _game.IsRunning = !_game.IsRunning;
            CursorState = CursorState == CursorState.Grabbed ? CursorState.Normal : CursorState.Grabbed;
            return;
        }

        if (_game.IsRunning)
        {
            _game.KeyDown(e.Key);
            if (e.Key == Keys.T)
            {
                CursorState = CursorState.Normal;
                Command();
                CursorState = CursorState.Grabbed;
            }
        }
        else
            _mainMenu.KeyDown(e);
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
        else
            _mainMenu.Click();
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
        if (!e.IsFocused)
        {
            _game.IsRunning = false;
            CursorState = CursorState.Normal;
        }
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
        => DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss");
}