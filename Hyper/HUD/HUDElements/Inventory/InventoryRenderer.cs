using System.Drawing;
using System.Drawing.Drawing2D;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using Svg;

namespace Hyper.HUD.HUDElements.Inventory;

internal class InventoryRenderer : IHudElement
{
    public bool Visible { get; set; } = true;

    private static readonly Vector2 HotbarSize = new(0.4f, 0.05f);

    private static readonly Vector2 HotbarPosition = new(0.0f, -0.38f);
    
    private readonly Texture _spriteSheet = Texture.LoadFromFile("HUD/HUDElements/Inventory/Resources/inventory.png");

    private readonly Dictionary<string, Vector4> _rectangles;

    public InventoryRenderer()
    {
        _rectangles = GetSvgRectanglesData("HUD/HUDElements/Inventory/Resources/inventory.svg");
    }

    public void Render(Shader shader)
    {
        GL.BindVertexArray(SharedVao.Instance.Vao);
        shader.SetBool("useTexture", true);
        _spriteSheet.Use(TextureUnit.Texture0);
        
        RenderHotbar(shader);
    }

    private void RenderHotbar(Shader shader)
    {
        var model = Matrix4.CreateTranslation(HotbarPosition.X, HotbarPosition.Y, 0.0f);
        model = Matrix4.CreateScale(HotbarSize.X, HotbarSize.Y, 1.0f) * model;
        shader.SetMatrix4("model", model);
        shader.SetVector4("spriteRect", _rectangles["hotbar"]);
        GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
        
        RenderHotbarItems(shader);
    }

    private void RenderHotbarItems(Shader shader)
    {
        var hotbar = PlayerData.InventorySystem.Inventory.Instance.Hotbar;
        var hotbarRect = _rectangles["hotbar"];
        for (int i = 0; i < 10; i++)
        {
            var item = hotbar[i].Item;
            if (item is not null)
                RenderHotbarItem(shader, item.ID, i, hotbarRect);
        }
    }
    
    private void RenderHotbarItem(Shader shader, string itemId, int index, Vector4 hotbarRect)
    {
        shader.SetVector4("spriteRect", _rectangles[itemId]);

        var rect = _rectangles[$"hotbar-{index}"];
        Matrix4 model = 
            Matrix4.CreateScale(HotbarSize.X * rect.Z / hotbarRect.Z, HotbarSize.Y * rect.W / hotbarRect.W, 1) * 
            Matrix4.CreateTranslation((hotbarRect.X - rect.X) * HotbarSize.X + HotbarPosition.X, 
                (hotbarRect.Y - rect.Y) * HotbarSize.Y + HotbarPosition.Y, 0);
        shader.SetMatrix4("model", model);
        GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
    }
    
    private Dictionary<string, Vector4> GetSvgRectanglesData(string filePath)
    {
        var svg = SvgDocument.Open(filePath);
        var result = new Dictionary<string, Vector4>();
        var offset = GetSvgOffset(svg);

        foreach (var element in svg.Children)
        {
            ParseSvgElements(element, result, svg.Width.Value, svg.Height.Value, offset);
        }

        return result;
    }
    
    private void ParseSvgElements(SvgElement element, Dictionary<string, Vector4> result, float width, float height, Vector2 offset)
    {
        var currentOffset = offset + GetSvgOffset(element);
        if (element is SvgVisualElement visualElement)
        {
            var bounds = visualElement.Bounds;
            result[visualElement.ID] = new Vector4(
                (bounds.X + currentOffset.X) / width, 
                (1.0f - (bounds.Y + currentOffset.Y + bounds.Height)) / height, 
                bounds.Width / width, 
                bounds.Height / height);
        }
        
        foreach (var child in element.Children)
        {
            ParseSvgElements(child, result, width, height, currentOffset);
        }
    }

    private Vector2 GetSvgOffset(SvgElement element)
    {
        if (element.Transforms is null) return new Vector2(0, 0);
        var transform = element.Transforms.GetMatrix();
        return new Vector2(transform.OffsetX, transform.OffsetY);
    }
}