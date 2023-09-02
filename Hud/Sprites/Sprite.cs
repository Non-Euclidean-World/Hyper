using System.Text.Json.Serialization;

namespace Hud.Sprites;

[Serializable]
public class Sprite
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("x")]
    public int X { get; set; } // Row in which the sprite is located.

    [JsonPropertyName("y")]
    public int Y { get; set; } // Column in which the sprite is located.

    [JsonPropertyName("width")]
    public int Width { get; set; } // Number of rows the sprite spans.

    [JsonPropertyName("height")]
    public int Height { get; set; } // Number of rows the sprite spans.
}