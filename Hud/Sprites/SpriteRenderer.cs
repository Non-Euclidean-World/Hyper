using System.Text.Json;
using Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Hud.Sprites;
/// <summary>
/// Renders sprites.
/// </summary>
public class SpriteRenderer
{
    private readonly Texture _spriteSheet;

    private readonly Dictionary<string, Vector4> _rectangles;

    private readonly int _spriteSheetWidth;

    private readonly int _spriteSheetHeight;

    /// <summary>
    /// Creates an instance of the <see cref="SpriteRenderer"/> class.
    /// </summary>
    /// <param name="metadataPath">The path to the JSON file with the sprite sheet metadata.</param>
    /// <param name="spriteSheetPath">The path to the PNG file with the sprite sheet.</param>
    public SpriteRenderer(string metadataPath, string spriteSheetPath)
    {
        using FileStream stream = File.OpenRead(metadataPath);
        var data = JsonSerializer.Deserialize<SpriteSheetMetadata>(stream);
        (_rectangles, _spriteSheetWidth, _spriteSheetHeight) = data!.GetRectangles();
        _spriteSheet = Texture.LoadFromFile(spriteSheetPath);
    }

    /// <summary>
    /// Sets up the shader to use the sprite sheet.
    /// </summary>
    /// <param name="shader">The shader that is used for rendering sprites.</param>
    public void UseTexture(Shader shader)
    {
        shader.SetBool("useTexture", true);
        _spriteSheet.Use(TextureUnit.Texture0);
    }

    /// <summary>
    /// Renders a sprite with the middle at a given position.
    /// </summary>
    /// <param name="shader">The shader that is used for rendering sprites.</param>
    /// <param name="spriteName">Name of the sprite.</param>
    /// <param name="position">Position at which the sprite is rendered.</param>
    /// <param name="sizeY">The height of the rendered sprite on the screen.</param>
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
    /// Renders a sprite which is rendered based on the position of its parent.
    /// </summary>
    /// <param name="shader">The shader that is used for rendering sprites.</param>
    /// <param name="spriteName">Name of the sprite we want to print.</param>
    /// <param name="relativePosition">The cell of the parent in which we want to render the object. If the parent is a 3 by 3 square and we want to render the object in the bottom left we pass [0,2].</param>
    /// <param name="parentPosition">The position at which the parent is rendered.</param>
    /// <param name="parentSizeY">The size on the screen of the parent sprite.</param>
    /// <param name="parentSpriteName">The name of the parent sprite.</param>
    public void RenderRelative(Shader shader, string spriteName, Vector2i relativePosition, Vector2 parentPosition, float parentSizeY, string parentSpriteName)
    {
        Vector2 position = GetPositionRelative(relativePosition, parentPosition, parentSpriteName);
        Vector2 size = GetSizeRelative(spriteName, parentSizeY, parentSpriteName);

        var model = Matrix4.CreateTranslation(position.X, position.Y, 0.0f);
        model = Matrix4.CreateScale(size.X, size.Y, 1.0f) * model;
        shader.SetMatrix4("model", model);
        shader.SetVector4("spriteRect", _rectangles[spriteName]);
        GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
    }

    /// <summary>
    /// Gets the position of a sprite which is rendered based on the position of its parent in world coordinates.
    /// </summary>
    /// <param name="relativePosition">The cell of the parent in which we want to render the object. If the parent is a 3 by 3 square and we want to render the object in the bottom left we pass [0,2].</param>
    /// <param name="parentPosition">The position at which the parent is rendered.</param>
    /// <param name="parentSpriteName">The name of the parent sprite.</param>
    /// <returns>The position of the sprite on the screen.</returns>
    public Vector2 GetPositionRelative(Vector2i relativePosition, Vector2 parentPosition, string parentSpriteName)
    {
        var parentRect = _rectangles[parentSpriteName];

        var positionX = parentPosition.X + ((2.0f * relativePosition.X + 1) / _spriteSheetWidth - parentRect.Z) / 2;
        var positionY = parentPosition.Y + ((2.0f * relativePosition.Y + 1) / _spriteSheetHeight - parentRect.W) / 2;

        return new Vector2(positionX, positionY);
    }

    /// <summary>
    /// Gets sprite size based on the size of the parent.
    /// </summary>
    /// <param name="spriteName"></param>
    /// <param name="parentSizeY"></param>
    /// <param name="parentSpriteName"></param>
    /// <returns></returns>
    private Vector2 GetSizeRelative(string spriteName, float parentSizeY, string parentSpriteName)
    {
        var parentRect = _rectangles[parentSpriteName];
        var rect = _rectangles[spriteName];

        var sizeY = parentSizeY * rect.W / parentRect.W;
        var sizeX = sizeY * rect.Z / rect.W;

        return new Vector2(sizeX, sizeY);
    }

    public void Dispose()
    {
        _spriteSheet.Dispose();
    }
}