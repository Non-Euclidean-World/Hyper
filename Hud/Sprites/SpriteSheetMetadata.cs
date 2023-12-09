using System.Text.Json.Serialization;
using OpenTK.Mathematics;

namespace Hud.Sprites;
/// <summary>
/// Represents a sprite sheet metadata.
/// </summary>
[Serializable]
public class SpriteSheetMetadata
{
    /// <summary>
    /// Width of the sprite sheet.
    /// </summary>
    [JsonPropertyName("width")]
    public int Width { get; set; }

    /// <summary>
    /// Height of the sprite sheet.
    /// </summary>
    [JsonPropertyName("height")]
    public int Height { get; set; }

    /// <summary>
    /// Items in the sprite sheet.
    /// </summary>
    [JsonPropertyName("items")]
    public List<Sprite> Items { get; set; } = null!;

    /// <summary>
    /// Gets the rectangles of the sprites in the sprite sheet. Rectangles describe the position of a sprite in the sprite sheet.
    /// </summary>
    /// <returns></returns>
    public (Dictionary<string, Vector4>, int, int) GetRectangles()
    {
        var result = new Dictionary<string, Vector4>();

        foreach (var item in Items)
        {
            var width = (float)item.Width / Width;
            var height = (float)item.Height / Height;
            var x = (float)item.X / Width;
            var y = 1.0f - (float)item.Y / Height - height;

            result.Add(item.Name, new Vector4(x, y, width, height));
        }

        return (result, Width, Height);
    }
}