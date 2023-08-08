using System.Text.Json;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Hyper.HUD.HUDElements.Sprites;

internal class SpriteRenderer
{
    private Dictionary<string, Vector4> _rectangles;

    private int spriteSheetWidth;
    
    private int spriteSheetHeight;
    
    public SpriteRenderer(string metadataPath)
    {
        using FileStream stream = File.OpenRead(metadataPath);
        var data = JsonSerializer.Deserialize<SpriteSheetMetadata>(stream);
        (_rectangles, spriteSheetWidth, spriteSheetHeight) = data!.GetRectangles();
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

    public void RenderRelative(Shader shader, string spriteName, Vector2i relativePosition, Vector2 parentPosition, float parentSizeY, string parentSpriteName)
    {
        var parentRect = _rectangles[parentSpriteName];
        var parentSizeX = parentSizeY * parentRect.Z / parentRect.W;

        var rect = _rectangles[spriteName];
        var positionX = parentPosition.X + (relativePosition.X - (parentRect.Z - 1.0f / spriteSheetWidth)) * parentSizeX;
        var positionY = parentPosition.Y + (relativePosition.Y - (parentRect.W - 1.0f / spriteSheetHeight)) * parentSizeY;
        var sizeY = parentSizeY * rect.W / parentRect.W;
        
        var model = Matrix4.CreateTranslation(positionX, positionY, 0.0f);
        model = Matrix4.CreateScale(sizeY * rect.Z / rect.W, sizeY, 1.0f) * model;
        shader.SetMatrix4("model", model);
        shader.SetVector4("spriteRect", rect);
        GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
    }
}