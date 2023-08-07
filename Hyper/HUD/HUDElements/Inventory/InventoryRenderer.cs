using System.Drawing;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Hyper.HUD.HUDElements.Inventory;

internal class InventoryRenderer : IHudElement
{
    public bool Visible { get; set; } = true;

    private static readonly Vector2 HotbarSize = new(0.4f, 0.05f);

    private static readonly Vector2 HotbarPosition = new(0.0f, -0.38f);
    
    private readonly Texture _spriteSheet = Texture.LoadFromFile("HUD/HUDElements/Inventory/Resources/Hotbar.png");

    public void Render(Shader shader)
    {
        GL.BindVertexArray(SharedVao.Instance.Vao);
        shader.SetBool("useTexture", true);
        _spriteSheet.Use(TextureUnit.Texture0);
        
        RenderHotbar(shader);
    }

    private static void RenderHotbar(Shader shader)
    {
        var model = Matrix4.CreateTranslation(HotbarPosition.X, HotbarPosition.Y, 0.0f);
        model = Matrix4.CreateScale(HotbarSize.X, HotbarSize.Y, 1.0f) * model;
        shader.SetMatrix4("model", model);
        shader.SetVector4("spriteRect", new Vector4(0, 0, 1, 1));
        
        GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
    }
}