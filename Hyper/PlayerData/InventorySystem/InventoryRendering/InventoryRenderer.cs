using Common;
using Hud;
using Hud.Sprites;
using Hyper.Controllers;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Hyper.PlayerData.InventorySystem.InventoryRendering;

internal class InventoryRenderer : IHudElement
{
    public bool Visible { get; set; } = true;

    public const float HotbarSizeY = 0.05f;

    public static readonly Vector2 HotbarPosition = new(0.0f, -0.38f);

    public static readonly Vector2 InventoryPosition = new(0.0f, 0.0f);
    
    private readonly Inventory _inventory = Inventory.Instance;
    
    private readonly SpriteRenderer _spriteRenderer = 
        new("PlayerData/InventorySystem/InventoryRendering/Resources/sprite_sheet.json", 
            "PlayerData/InventorySystem/InventoryRendering/Resources/sprite_sheet.png");

    public void Render(Shader shader)
    {
        GL.BindVertexArray(SharedVao.Instance.Vao);
        _spriteRenderer.UseTexture(shader);
        
        shader.SetVector4("color", Vector4.One);

        RenderHotbar(shader);
        RenderInventory(shader);
        RenderInHandItem(shader);
        
        shader.SetVector4("color", new Vector4(0, 0, 0, 1));
        RenderItemCounts(shader);
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
    
    private void RenderItemCounts(Shader shader)
    {
        RenderHotbarItemCounts(shader);
        if (_inventory.IsOpen) RenderInventoryItemCounts(shader);
    }

    private void RenderHotbarItemCounts(Shader shader)
    {
        var hotbar = _inventory.Hotbar;
        for (int i = 0; i < 10; i++)
        {
            var item = hotbar[i].Item;
            if (item is not null && item.IsStackable)
                RenderItemCount(shader, hotbar[i].Count,new Vector2i(i, 0), HotbarPosition, "hotbar");
        }
    }

    private void RenderInventoryItemCounts(Shader shader)
    {
        var inventory = _inventory.Items;
        for (int i = 0; i < 10; i++)
        {
            for (int j = 1; j < 4; j++)
            {
                var item = inventory[i, j].Item;
                if (item is not null && item.IsStackable)
                    RenderItemCount(shader, inventory[i, j].Count ,new Vector2i(i, Inventory.Rows - j - 1), InventoryPosition, "inventory");
            }
        }
    }

    private void RenderItemCount(Shader shader, int count, Vector2i relativePosition, Vector2 parentPosition, string parentSpriteId)
    {
        var position = _spriteRenderer.GetPositionRelative(relativePosition, parentPosition, parentSpriteId);
        Printer.RenderStringBottomRight(shader, count.ToString(), 0.2f * HotbarSizeY, position.X + 0.6f * HotbarSizeY, position.Y - 0.6f * HotbarSizeY);
    }

    private void RenderInHandItem(Shader shader)
    {
        if (_inventory.InHandItem.Item is null) return;

        _spriteRenderer.Render(shader, _inventory.InHandItem.Item.ID, HudController.GetMousePosition(), HotbarSizeY);
    }
}