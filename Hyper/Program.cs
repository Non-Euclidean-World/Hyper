using Hyper;
using Hyper.Animation;
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

var test = new Model();

using (var window = new Window(GameWindowSettings.Default, nativeWindowSettings))
{
    window.RenderFrequency = 500.0f;
    window.Run();
}

