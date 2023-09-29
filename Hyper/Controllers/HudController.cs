using Common;
using Common.UserInput;
using Hud;
using Hud.HUDElements;
using Hud.Shaders;
using Hyper.PlayerData.InventorySystem.InventoryRendering;
using Hyper.PlayerData.InventorySystem.Items;
using Hyper.Shaders;
using OpenTK.Graphics.OpenGL4;

namespace Hyper.Controllers;

internal class HudController : IController
{
    private readonly Scene _scene;
    
    private readonly HudShader _shader;

    private readonly IWindowHelper _windowHelper;

    public HudController(Scene scene, IWindowHelper windowHelper, HudShader shader)
    {
        _scene = scene;
        _windowHelper = windowHelper;
        _shader = shader;
    }

    public void Render()
    {
        GL.Disable(EnableCap.DepthTest);
        _shader.SetUp(_windowHelper.GetAspectRatio());

        foreach (var element in _scene.HudElements)
        {
            if (element is Crosshair && _scene.Player.Inventory.SelectedItem is Hammer) continue;
            if (element.Visible) element.Render(_shader);
        }
        GL.Enable(EnableCap.DepthTest);
    }

    public void Dispose()
    {
        _shader.Dispose();
    }
}
