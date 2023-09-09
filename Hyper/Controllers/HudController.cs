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

    private readonly HudHelper _hudHelper;

    public HudController(HudHelper hudHelper, HudShader shader)
    {
        _hudHelper = hudHelper;
        _shader = shader;
        _elements = new IHudElement[]
        {
            new Crosshair(),
            new FpsCounter(_hudHelper),
            new InventoryHudManager(_hudHelper),
        };
    }

    public void Render()
    {
        GL.Disable(EnableCap.DepthTest);
        _shader.SetUp(_hudHelper.GetAspectRatio());

        foreach (var element in _elements)
        {
            if (element.Visible) element.Render(_shader);
        }
        GL.Enable(EnableCap.DepthTest);
    }
}
