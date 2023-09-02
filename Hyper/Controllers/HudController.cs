using Common;
using Hud;
using Hud.HUDElements;
using Hyper.Shaders;
using OpenTK.Graphics.OpenGL4;
using Player.InventorySystem.InventoryRendering;

namespace Hyper.Controllers;

internal class HudController : IController
{
    private readonly Shader _shader;

    private readonly IHudElement[] _elements;

    private readonly HudHelper _hudHelper;

    // TODO fix fps counter and fix the offset on item moving.
    public HudController(HudHelper hudHelper, Shader shader)
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
        ShaderFactory.SetUpHudShaderParams(_shader, _hudHelper.GetAspectRatio());

        foreach (var element in _elements)
        {
            if (element.Visible) element.Render(_shader);
        }
        GL.Enable(EnableCap.DepthTest);
    }
}
