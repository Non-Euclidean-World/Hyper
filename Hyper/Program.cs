using Common;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace Hyper;

internal class Program
{
    private static void Main(string[] args)
    {
        const int height = 600;
        var nativeWindowSettings = new NativeWindowSettings()
        {
            Size = new Vector2i(height * 16 / 9, height),
            Title = "Hyper",
            // This is needed to run on macos
            Flags = ContextFlags.ForwardCompatible,
        };

        GeometryType geometryType = default;
        if (args.Length > 0)
        {
            geometryType = Enum.Parse<GeometryType>(args[0], ignoreCase: true);
        }
        using var window = new Window(GameWindowSettings.Default, nativeWindowSettings, geometryType);
        window.RenderFrequency = 60.0f; // TODO we *really* need to rein this thing in. My CPU fries otherwise
        window.Run();
    }
}