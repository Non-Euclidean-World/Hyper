using System.Drawing;
using System.Drawing.Drawing2D;
using Hyper.HUD.HUDElements.Sprites;
using Hyper.PlayerData.InventorySystem.Items;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Svg;

namespace Hyper.HUD.HUDElements.Inventory;

internal class InventoryRenderer : IHudElement
{
    public bool Visible { get; set; } = true;

    private static readonly float HotbarSize = 0.05f;

    private static readonly Vector2 HotbarPosition = new(0.0f, -0.38f);
    
    private readonly Texture _spriteSheet = Texture.LoadFromFile("HUD/HUDElements/Inventory/Resources/sprite_sheet.png");

    private SpriteRenderer _spriteRenderer = new("HUD/HUDElements/Inventory/Resources/sprite_sheet.json");

    public void Render(Shader shader)
    {
        GL.BindVertexArray(SharedVao.Instance.Vao);
        shader.SetBool("useTexture", true);
        _spriteSheet.Use(TextureUnit.Texture0);
        
        RenderHotbar(shader);
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
            new Vector2i(PlayerData.InventorySystem.Inventory.Instance.SelectedItemIndex, 0), 
            HotbarPosition, 
            HotbarSize, 
            "hotbar");
    }

    private void RenderHotbarItems(Shader shader)
    {
        var hotbar = PlayerData.InventorySystem.Inventory.Instance.Hotbar;
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
}