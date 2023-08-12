using Hyper;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

var nativeWindowSettings = new NativeWindowSettings()
{
    Size = new Vector2i(800, 600),
    Title = "Hyper",
    // This is needed to run on macos
    Flags = ContextFlags.ForwardCompatible,
};

using (var window = new Window(GameWindowSettings.Default, nativeWindowSettings))
{
    window.RenderFrequency = 60.0f; // TODO we *really* need to rein this thing in. My CPU fries otherwise
    window.Run();
}

