using System.Text.Json.Serialization;

namespace Hud.Sprites;
/// <summary>
/// A description of a sprite.
/// </summary>
[Serializable]
public class Sprite
{
    /// <summary>
    /// The name of the sprite.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    /// <summary>
    /// The row in which the sprite is location in the sprite sheet.
    /// </summary>
    [JsonPropertyName("x")]
    public int X { get; set; }

    /// <summary>
    /// The column in which the sprite is location in the sprite sheet.
    /// </summary>
    [JsonPropertyName("y")]
    public int Y { get; set; }

    /// <summary>
    /// The width of the sprite in the sprite sheet.
    /// </summary>
    [JsonPropertyName("width")]
    public int Width { get; set; }

    /// <summary>
    /// The height of the sprite in the sprite sheet.
    /// </summary>
    [JsonPropertyName("height")]
    public int Height { get; set; }
}