using Hyper;
using Hyper.MarchingCubes;
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

using (var window = new MarchingCubesWindow(GameWindowSettings.Default, nativeWindowSettings))
{
    window.RenderFrequency = 30.0f;
    window.Run();
}

