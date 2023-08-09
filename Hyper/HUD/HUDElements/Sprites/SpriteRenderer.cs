using System.Text.Json;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Hyper.HUD.HUDElements.Sprites;

internal class SpriteRenderer
{
    private readonly Texture _spriteSheet;
    
    private readonly Dictionary<string, Vector4> _rectangles;

    private readonly int _spriteSheetWidth;
    
    private readonly int _spriteSheetHeight;
    
    public SpriteRenderer(string metadataPath, string spriteSheetPath)
    {
        using FileStream stream = File.OpenRead(metadataPath);
        var data = JsonSerializer.Deserialize<SpriteSheetMetadata>(stream);
        (_rectangles, _spriteSheetWidth, _spriteSheetHeight) = data!.GetRectangles();
        _spriteSheet = Texture.LoadFromFile(spriteSheetPath);
    }
    
    public void UseTexture(Shader shader)
    {
        shader.SetBool("useTexture", true);
        _spriteSheet.Use(TextureUnit.Texture0);
    }
    
    public void Render(Shader shader, string spriteName, Vector2 position, float sizeY)
    {
        var rect = _rectangles[spriteName];
        var model = Matrix4.CreateTranslation(position.X, position.Y, 0.0f);
        model = Matrix4.CreateScale(sizeY * rect.Z / rect.W, sizeY, 1.0f) * model;
        shader.SetMatrix4("model", model);
        shader.SetVector4("spriteRect", rect);
        GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
    }

    /// <summary>
    /// Renders a sprite relative to another sprite. I hope it works.
    /// </summary>
    /// <param name="shader"></param>
    /// <param name="spriteName">Name of the sprite we want to print.</param>
    /// <param name="relativePosition">The cell of the parent in which we want to render the object. If parent is a 3 by 3 square and we want to render the object in the bottom left we pass [0,2].</param>
    /// <param name="parentPosition">The position at which the parent is rendered.</param>
    /// <param name="parentSizeY">Parents Y size.</param>
    /// <param name="parentSpriteName">Parent sprite name.</param>
    public void RenderRelative(Shader shader, string spriteName, Vector2i relativePosition, Vector2 parentPosition, float parentSizeY, string parentSpriteName)
    {
        var parentRect = _rectangles[parentSpriteName];
        var rect = _rectangles[spriteName];
        
        var positionX = parentPosition.X + ((2.0f * relativePosition.X + 1) / _spriteSheetWidth - parentRect.Z) / 2;
        var positionY = parentPosition.Y + ((2.0f * relativePosition.Y + 1) / _spriteSheetHeight - parentRect.W) / 2;
        var sizeY = parentSizeY * rect.W / parentRect.W;
        
        var model = Matrix4.CreateTranslation(positionX, positionY, 0.0f);
        model = Matrix4.CreateScale(sizeY * rect.Z / rect.W, sizeY, 1.0f) * model;
        shader.SetMatrix4("model", model);
        shader.SetVector4("spriteRect", rect);
        GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
    }
}