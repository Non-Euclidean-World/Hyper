using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace Hud.Menu.Colors;
internal static class ColorGetter
{
    public static Vector4 GetVector(Color color) => color switch
    {
        Color.White => new Vector4(1, 1, 1, 1),
        Color.Black => new Vector4(0, 0, 0, 1),
        Color.Red => new Vector4(1, 0, 0, 1),
        Color.Green => new Vector4(0, 1, 0, 1),
        Color.Blue => new Vector4(0, 0, 1, 1),
        _ => Vector4.One,
    };
}
