using OpenTK.Mathematics;

namespace Hyper.HUD.HUDElements.Inventory;

internal class InventoryRenderer : HudElement
{
    private Texture _spriteSheet;

    public InventoryRenderer(Vector2 position, float size) : base(position, size)
    {
        _spriteSheet = Texture.LoadFromFile("HUD/HUDElements/Inventory/Resources/Hotbar.png");
    }

    public override void Render(Shader shader)
    {
        throw new NotImplementedException();
    }
}