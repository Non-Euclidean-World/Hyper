using Common;
using Hud.Menu;
using Hud.Shaders;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Hyper;

public class Menu
{
    private readonly IWindowHelper _windowHelper;

    private HudShader _shader = HudShader.Create();
    
    private IWidget _menu = new Column(
        children: new IWidget[]
        {
            new Row(
                children: new IWidget[]
                {
                    new Text("row1col1", 0.02f),
                    new Text("row2col1", 0.02f),
                    new Text("row3col1", 0.02f),
                }
            ),
            new Row(
                children: new IWidget[]
                {
                    new Text("row1col1", 0.02f),
                    new Text("row2col1", 0.02f),
                    new Text("row3col1", 0.02f),
                }
            ),
            new Row(
                children: new IWidget[]
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
        var aspectRatio = _windowHelper.GetAspectRatio();
        _shader.SetUp(aspectRatio);
        _shader.SetBool("useTexture", false);
        _menu.Render(new Context(_shader, Vector2.Zero, new Vector2(aspectRatio, 1)));
        GL.Enable(EnableCap.DepthTest);
    }
}