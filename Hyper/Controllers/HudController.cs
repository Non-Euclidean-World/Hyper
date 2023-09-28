using Common;
using Common.UserInput;
using Hud;
using Hud.HUDElements;
using Hud.Shaders;
using Hyper.Shaders;
using OpenTK.Graphics.OpenGL4;
using Player.InventorySystem.InventoryRendering;

namespace Hyper.Controllers;

internal class HudController : IController
{
    private readonly HudShader _shader;

    private readonly IHudElement[] _elements;

    private readonly IWindowHelper _windowHelper;

    public HudController(Scene scene, Context context, IWindowHelper windowHelper, HudShader shader)
    {
        _windowHelper = windowHelper;
        _shader = shader;
        _elements = new IHudElement[]
        {
            new Crosshair(){ Visible = false},
            new FpsCounter(_windowHelper),
            new InventoryHudManager(_windowHelper, scene.Player.Inventory, context),
        };
    }

    public void Render()
    {
        GL.Disable(EnableCap.DepthTest);
        _shader.SetUp(_windowHelper.GetAspectRatio());

        foreach (var element in _elements)
        {
            if (element.Visible) element.Render(_shader);
        }
        GL.Enable(EnableCap.DepthTest);
    }

    public void Dispose()
    {
        foreach (var element in _elements)
        {
            element.Dispose();
        }

        _shader.Dispose();
    }
}
