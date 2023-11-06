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
    private readonly IWindowHelper _windowHelper;

    private readonly HudShader _shader = HudShader.Create();
    
    private readonly Widget _menu = new Column(
        children: new Widget[]
        {
            new Row(
                children: new Widget[]
                {
                    new Center(new TextBox("row1col1", 0.02f, Color.Green)),
                    new Center(new Center(new TextBox("row2col1", 0.02f))),
                    new Background(Color.Red, new TextBox("row3col1", 0.02f)),
                }
            ),
            new Row(
                children: new Widget[]
                {
                    new TextBox("test", 0.02f),
                    new InputTextBox("inputtext", 0.02f),
                    new Background(Color.Blue, new Padding(new Background(Color.Red, new TextBox("row3col1", 0.02f)), 0.01f)),
                }
            ),
            new Row(
                children: new Widget[]
                {
                    new TextBox("row1col1", 0.02f),
                    new TextBox("row2col1", 0.02f),
                    new TextBox("row3col1", 0.02f),
                }
            )
        });

    private readonly Widget _savedGames = new SavedGames();

    public MainMenu(IWindowHelper windowHelper)
    {
        _windowHelper = windowHelper;
    }
    
    public void Render()
    {
        GL.Disable(EnableCap.DepthTest);
        GL.Clear(ClearBufferMask.ColorBufferBit);
        var aspectRatio = _windowHelper.GetAspectRatio();
        _shader.SetUp(aspectRatio);
        _shader.UseTexture(false);
        _shader.SetColor(Vector4.One);
        _savedGames.Render(new Context(_shader, new Vector2(-aspectRatio / 2, 0.5f), new Vector2(aspectRatio, 1)));
        GL.Enable(EnableCap.DepthTest);
    }
    
    public void Click()
    {
        _menu.Click(_windowHelper.GetMousePosition());
    }
    
    public void KeyDown(Keys key)
    {
        _menu.KeyboardInput(key);
    }
}