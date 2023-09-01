using System.Text.Json.Serialization;
using OpenTK.Mathematics;

namespace Hud.Sprites;

[Serializable]
public class SpriteSheetMetadata
{
    [JsonPropertyName("width")]
    public int Width { get; set; }
    
    [JsonPropertyName("height")]
    public int Height { get; set; }

    [JsonPropertyName("items")] 
    public List<Sprite> Items { get; set; } = null!;

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