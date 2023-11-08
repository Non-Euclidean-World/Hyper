using OpenTK.Mathematics;

namespace Hud.Widgets.Colors;
public static class ColorGetter
{
    public static Vector4 GetVector(Color color) => color switch
    {
        Color.Primary => new Vector4(0, 0, 1, 1),
        Color.Secondary => new Vector4(1, 1, 1, 1),
        Color.Background => new Vector4(0.9f, 0.7f, 0.7f, 1),

        Color.White => new Vector4(1, 1, 1, 1),
        Color.Black => new Vector4(0, 0, 0, 1),
        Color.Red => new Vector4(1, 0, 0, 1),
        Color.Green => new Vector4(0, 1, 0, 1),
        Color.Blue => new Vector4(0, 0, 1, 1),
        _ => Vector4.One,
    };
}
