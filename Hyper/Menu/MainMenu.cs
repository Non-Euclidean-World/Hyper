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
    
    private readonly IWindowHelper _windowHelper;

    private readonly HudShader _shader = HudShader.Create();

    private Widget _activeWidget;
    
    private readonly MainMenuScreen _menu = new ();
    
    private readonly LoadGame _loadGame = new ();

    public MainMenu(IWindowHelper windowHelper)
    {
        _windowHelper = windowHelper;
        _activeWidget = _menu;
        _menu.Resume += () => Resume?.Invoke();
        _menu.Load += () => _activeWidget = _loadGame;
        _menu.Quit += () => Quit?.Invoke();
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