using Common;
using Hud.Menu;
using Hud.Menu.Colors;
using Hud.Shaders;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Hyper;

public class Menu
{
    private readonly IWindowHelper _windowHelper;

    private readonly HudShader _shader = HudShader.Create();
    
    private readonly Widget _menu = new Column(
        children: new Widget[]
        {
            new Row(
                children: new Widget[]
                {
                    new Center(new Text("row1col1", 0.02f)),
                    new Center(new Center(new Text("row2col1", 0.02f))),
                    new Background(Color.Red, new Text("row3col1", 0.02f)),
                }
            ),
            new Row(
                children: new Widget[]
                {
                    new Text("test", 0.02f),
                    new InputText("inputtext", 0.02f),
                    new Text("row3col1", 0.02f),
                }
            ),
            new Row(
                children: new Widget[]
                {
                    new Text("row1col1", 0.02f),
                    new Text("row2col1", 0.02f),
                    new Text("row3col1", 0.02f),
                }
            )
        });

    public Menu(IWindowHelper windowHelper)
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
        _menu.Render(new Context(_shader, new Vector2(-aspectRatio / 2, 0.5f), new Vector2(aspectRatio, 1)));
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