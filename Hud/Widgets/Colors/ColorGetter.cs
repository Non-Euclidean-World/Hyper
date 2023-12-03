using OpenTK.Mathematics;

namespace Hud.Widgets.Colors;
/// <summary>
/// Converts Color to Vector4.
/// </summary>
public static class ColorGetter
{
    public static Vector4 GetVector(Color color) => color switch
    {
        Color.Primary => new Vector4(0.15f, 0.22f, 0.35f, 1),
        Color.Secondary => new Vector4(0.72f, 0.55f, 0.22f, 1),
        Color.Background => new Vector4(0.69f, 0.73f, 0.75f, 1),

        Color.White => new Vector4(1, 1, 1, 1),
        Color.Black => new Vector4(0, 0, 0, 1),
        Color.Red => new Vector4(1, 0, 0, 1),
        Color.Green => new Vector4(0, 1, 0, 1),
        Color.Blue => new Vector4(0, 0, 1, 1),
        _ => Vector4.One,
    };
}
