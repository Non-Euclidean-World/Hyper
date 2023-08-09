using Hyper.HUD.Sprites;
using Hyper.PlayerData.InventorySystem;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Hyper.HUD.HUDElements.InventoryRendering;

internal class InventoryRenderer : IHudElement
{
    public bool Visible { get; set; } = true;

    public const float HotbarSizeY = 0.05f;

    public static readonly Vector2 HotbarPosition = new(0.0f, -0.38f);

    public static readonly Vector2 InventoryPosition = new(0.0f, 0.0f);
    
    private readonly Inventory _inventory = Inventory.Instance;
    
    private readonly SpriteRenderer _spriteRenderer = 
        new("HUD/HUDElements/InventoryRendering/Resources/sprite_sheet.json", 
            "HUD/HUDElements/InventoryRendering/Resources/sprite_sheet.png");

    public void Render(Shader shader)
    {
        GL.BindVertexArray(SharedVao.Instance.Vao);
        _spriteRenderer.UseTexture(shader);
        
        RenderHotbar(shader);
        RenderInventory(shader);
        RenderInHandItem(shader);
    }

    private void RenderHotbar(Shader shader)
    {
        _spriteRenderer.Render(shader, "hotbar", HotbarPosition, HotbarSizeY);

        RenderHotbarSelectedSlot(shader);
        RenderHotbarItems(shader);
    }

    private void RenderHotbarSelectedSlot(Shader shader)
    {
        _spriteRenderer.RenderRelative(
            shader, 
            "selectedSlot", 
            new Vector2i(_inventory.SelectedItemIndex, 0),
            HotbarPosition, 
            HotbarSizeY, 
            "hotbar");
    }

    private void RenderHotbarItems(Shader shader)
    {
        var hotbar = _inventory.Hotbar;
        for (int i = 0; i < 10; i++)
        {
            var item = hotbar[i].Item;
            if (item is not null)
                RenderHotbarItem(shader, item.ID, i);
        }
    }
    
    private void RenderHotbarItem(Shader shader, string itemId, int index)
    {
        _spriteRenderer.RenderRelative(
            shader, 
            itemId, 
            new Vector2i(index, 0), 
            HotbarPosition, 
            HotbarSizeY, 
            "hotbar");
    }
    
    private void RenderInventory(Shader shader)
    {
        if (!_inventory.IsOpen) return;
        
        _spriteRenderer.Render(shader, "inventory", InventoryPosition, 3 * HotbarSizeY);
        RenderInventoryItems(shader);
    }

    private void RenderInventoryItems(Shader shader)
    {
        var inventory = _inventory.Items;
        for (int i = 0; i < 10; i++)
        {
            for (int j = 1; j < 4; j++)
            {
                var item = inventory[i, j].Item;
                if (item is not null)
                    RenderInventoryItem(shader, item.ID, i, Inventory.Rows - j - 1);
            }
        }
    }

    private void RenderInventoryItem(Shader shader, string itemId, int x, int y)
    {
        _spriteRenderer.RenderRelative(
            shader, 
            itemId, 
            new Vector2i(x, y), 
            Vector2.Zero, 
            3 * HotbarSizeY, 
            "inventory");
    }

    private void RenderInHandItem(Shader shader)
    {
        if (_inventory.InHandItem is null) return;

        _spriteRenderer.Render(shader, _inventory.InHandItem.ID, HudManager.GetMousePosition(), HotbarSizeY);
    }
}