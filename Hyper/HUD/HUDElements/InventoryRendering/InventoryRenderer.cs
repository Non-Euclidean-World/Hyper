using Hyper.HUD.HUDElements.Sprites;
using Hyper.PlayerData.InventorySystem;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Hyper.HUD.HUDElements.InventoryRendering;

internal class InventoryRenderer : IHudElement
{
    public bool Visible { get; set; } = true;

    private const float HotbarSize = 0.05f;

    private static readonly Vector2 HotbarPosition = new(0.0f, -0.38f);
    
    private readonly Texture _spriteSheet = Texture.LoadFromFile("HUD/HUDElements/InventoryRendering/Resources/sprite_sheet.png");

    private readonly SpriteRenderer _spriteRenderer = new("HUD/HUDElements/InventoryRendering/Resources/sprite_sheet.json");

    public void Render(Shader shader)
    {
        GL.BindVertexArray(SharedVao.Instance.Vao);
        shader.SetBool("useTexture", true);
        _spriteSheet.Use(TextureUnit.Texture0);
        
        RenderHotbar(shader);
        RenderInventory(shader);
    }

    private void RenderHotbar(Shader shader)
    {
        _spriteRenderer.Render(shader, "hotbar", HotbarPosition, HotbarSize);

        RenderHotbarSelectedSlot(shader);
        RenderHotbarItems(shader);
    }

    private void RenderHotbarSelectedSlot(Shader shader)
    {
        _spriteRenderer.RenderRelative(
            shader, 
            "selectedSlot", 
            new Vector2i(Inventory.Instance.SelectedItemIndex, 0),
            HotbarPosition, 
            HotbarSize, 
            "hotbar");
    }

    private void RenderHotbarItems(Shader shader)
    {
        var hotbar = Inventory.Instance.Hotbar;
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
            HotbarSize, 
            "hotbar");
    }
    
    private void RenderInventory(Shader shader)
    {
        if (!Inventory.Instance.IsOpen) return;
        
        _spriteRenderer.Render(shader, "inventory", Vector2.Zero, 3 * HotbarSize);
        RenderInventoryItems(shader);
    }

    private void RenderInventoryItems(Shader shader)
    {
        var inventory = Inventory.Instance.Items;
        for (int i = 0; i < 10; i++)
        {
            for (int j = 1; j < 4; j++)
            {
                var item = inventory[i, j].Item;
                if (item is not null)
                    RenderInventoryItem(shader, item.ID, i, Inventory.ItemRows - j - 1);
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
            3 * HotbarSize, 
            "inventory");
    }
}