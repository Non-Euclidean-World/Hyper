using Common;
using Common.UserInput;
using Hud;
using Hud.HUDElements;
using Hud.Shaders;
using Hyper.PlayerData;
using Hyper.PlayerData.InventorySystem.InventoryRendering;
using Hyper.PlayerData.InventorySystem.Items.Pickaxes;
using OpenTK.Graphics.OpenGL4;

namespace Hyper.Controllers;

internal class HudController : IController
{
    private readonly Scene _scene;

    private readonly HudShader _shader;

    private readonly IWindowHelper _windowHelper;

    private readonly IHudElement[] _hudElements;

    public HudController(Scene scene, IWindowHelper windowHelper, HudShader shader, Context context)
    {
        _scene = scene;
        _windowHelper = windowHelper;
        _shader = shader;
        _hudElements = new IHudElement[]
        {
            new Crosshair(),
            new FpsCounter(windowHelper),
            new InventoryHudManager(windowHelper, _scene.Player.Inventory, context),
            new TipText("C to enter the car", () => _scene.TryEnterClosestCar(testOnly: true), top: 0.4f),
            new TipText("L to leave the car", () => _scene.PlayersCar != null, top: 0.3f),
            new TipText("F to flip the car", () => _scene.TryFlipClosestCar(testOnly: true), top: 0.2f),
            new TipText("P to pick the lamp", () => _scene.TryPickLamp(testOnly: true), top: 0.1f),
            new PositionPrinter(_scene.Camera, windowHelper)
        };
    }

    public void Render()
    {
        GL.Disable(EnableCap.DepthTest);
        _shader.SetUp(_windowHelper.GetAspectRatio());

        foreach (var element in _hudElements)
        {
            if (element is Crosshair &&
                _scene.Player.Inventory.SelectedItem is Pickaxe)
                continue;

            if (element.Visible)
                element.Render(_shader);
        }
        GL.Enable(EnableCap.DepthTest);
    }

    public void Dispose()
    {
        _shader.Dispose();
        foreach (var element in _hudElements)
        {
            element.Dispose();
        }
    }
}
