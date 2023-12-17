using Common;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace Hyper;

internal class Program
{
    private static void Main(string[] args)
    {
#if RELEASE
        string currentDir = Directory.GetCurrentDirectory();
        string logsDir = Path.Combine(currentDir, "logs");
        Directory.CreateDirectory(logsDir);
        DateTime gameStartedTime = DateTime.Now;
#endif
        const int height = 600;
        var nativeWindowSettings = new NativeWindowSettings()
        {
            Size = new Vector2i(height * 16 / 9, height),
            Title = "Hyper",
            // This is needed to run on macos
            Flags = ContextFlags.ForwardCompatible,
        };

        SelectedGeometryType selectedGeometryType = SelectedGeometryType.None;
        if (args.Length > 0)
        {
            selectedGeometryType = Enum.Parse<SelectedGeometryType>(args[0], ignoreCase: true);
        }
#if RELEASE
        try
#endif
        {
            using var window = new Window(GameWindowSettings.Default, nativeWindowSettings, selectedGeometryType);
            window.RenderFrequency = 60.0f; // TODO we *really* need to rein this thing in. My CPU fries otherwise

            window.Run();
        }
#if RELEASE
        catch (Exception ex)
        {
            DateTime failureTime = DateTime.Now;
            string fileName = DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss") + ".txt";
            string logFilePath = Path.Combine(logsDir, fileName);
            var timeRunning = failureTime - gameStartedTime;
            string errorMessage = $"Time running: {timeRunning}\n\n{ex}";

            File.WriteAllText(logFilePath, errorMessage);
        }
#endif
    }
}