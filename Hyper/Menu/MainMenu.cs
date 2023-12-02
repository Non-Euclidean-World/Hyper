using Common;
using Hud.Shaders;
using Hud.Widgets;
using Hud.Widgets.MultipleChildren;
using Hud.Widgets.SingleChild;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

namespace Hyper.Menu;
/// <summary>
/// The main menu of the game. It lets the user start a new game, load a game, delete a game, or quit the game.
/// </summary>
public class MainMenu
{
    private enum SaveGridMode
    {
        Load,
        Delete
    }

    public event Action Resume = null!;
    public event Action<string, GeometryType> NewGame = null!;
    public event Action<string> Load = null!;
    public event Action<string> Delete = null!;
    public event Action Quit = null!;

    private readonly IWindowHelper _windowHelper;

    private readonly HudShader _shader = HudShader.Create();

    private Widget _activeWidget;

    private readonly AppBar _appBar = new();

    private readonly SaveGrid _saveGrid;

    private readonly Widget _saveGridScreen;

    private readonly NewGame _newGame = new();

    private readonly Widget _newGameScreen;
    
    private readonly Widget _controlsScreen;

    private SaveGridMode _saveGridMode = SaveGridMode.Load;

    public MainMenu(IWindowHelper windowHelper)
    {
        _windowHelper = windowHelper;
        _activeWidget = _appBar;
        SetUpAppBar();
        (_saveGrid, _saveGridScreen) = GetSaveGrid();
        _newGameScreen = GetWidgetWrapped(_newGame);
        _newGame.Create += (saveName, geometryType) => NewGame.Invoke(saveName, geometryType);
        _controlsScreen = GetWidgetWrapped(new Controls());
    }

    public void Reload()
    {
        _saveGrid.Reload();
        _newGame.Reload();
    }

    private Widget GetWidgetWrapped(Widget widget)
    {
        return new Background(
        new Row(
                alignment: Alignment.Greedy,
                children: new[]
                {
                    _appBar,
                    widget
                }
            )
        );
    }

    private void SetUpAppBar()
    {
        _appBar.Resume += () =>
        {
            _activeWidget = _appBar;
            Resume.Invoke();
        };
        _appBar.Load += () =>
        {
            _saveGrid.Title = "Load Game";
            _saveGridMode = SaveGridMode.Load;
            _activeWidget = _saveGridScreen;
        };
        _appBar.Delete += () =>
        {
            _saveGrid.Title = "Delete Game";
            _saveGridMode = SaveGridMode.Delete;
            _activeWidget = _saveGridScreen;
        };
        _appBar.NewGame += () =>
        {
            _activeWidget = _newGameScreen;
        };
        _appBar.Controls += () =>
        {
            _activeWidget = _controlsScreen;
        };
        _appBar.Quit += () => Quit.Invoke();
    }

    private (SaveGrid, Widget) GetSaveGrid()
    {
        var saveGrid = new SaveGrid();
        saveGrid.OnSelected += (saveName) =>
        {
            switch (_saveGridMode)
            {
                case SaveGridMode.Load:
                    Load.Invoke(saveName);
                    break;
                case SaveGridMode.Delete:
                    Delete.Invoke(saveName);
                    break;
            }
        };
        var saveGridScreen = GetWidgetWrapped(saveGrid);

        return (saveGrid, saveGridScreen);
    }

    public void Render()
    {
        GL.Disable(EnableCap.DepthTest);
        GL.Clear(ClearBufferMask.ColorBufferBit);
        var aspectRatio = _windowHelper.GetAspectRatio();
        _shader.SetUp(aspectRatio);
        _shader.UseTexture(false);
        _shader.SetColor(Vector4.One);
        _activeWidget.Render(new Context(_shader, new Vector2(-aspectRatio / 2, 0.5f), new Vector2(aspectRatio, 1)));
        GL.Enable(EnableCap.DepthTest);
    }

    public void Click()
    {
        _activeWidget.Click(_windowHelper.GetMousePosition());
    }

    public void KeyDown(KeyboardKeyEventArgs key)
    {
        _activeWidget.KeyboardInput(key);
    }
}