using Common;
using Hud.Shaders;
using Hud.Widgets;
using Hud.Widgets.Colors;
using Hud.Widgets.MultipleChildren;
using Hud.Widgets.NoChildren;
using Hud.Widgets.SingleChild;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Hyper.Menu;

public class MainMenu
{
    public event Action Resume = null!;
    
    public event Action Quit = null!;

    public event Action<string> Load = null!;
    
    private readonly IWindowHelper _windowHelper;

    private readonly HudShader _shader = HudShader.Create();

    private Widget _activeWidget;
    
    private readonly AppBar _appBar = new ();
    
    private readonly Widget _loadGame;

    public MainMenu(IWindowHelper windowHelper)
    {
        _windowHelper = windowHelper;
        _activeWidget = _appBar;
        _appBar.Resume += () => 
        {
            _activeWidget = _appBar;
            Resume?.Invoke();
        };
        _appBar.Load += () => _activeWidget = _loadGame;
        _appBar.Quit += () => Quit?.Invoke();

        var loadGameWidget = new LoadGame();
        loadGameWidget.Load += (saveName) => Load?.Invoke(saveName);
        _loadGame = new Row(
            alignment: Alignment.Greedy,
            children: new Widget[]
            {
                _appBar,
                loadGameWidget
            }
        );
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
    
    public void KeyDown(Keys key)
    {
        _activeWidget.KeyboardInput(key);
    }
}