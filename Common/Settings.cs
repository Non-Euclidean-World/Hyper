using System.Text.Json;

namespace Common;

public class Settings
{
    public int Seed { get; private set; }

    public string SaveName { get; private set; }
    
    public float AspectRatio { get; private set; }

    private static readonly string AppDataLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Hyper");

    public static readonly string SavesLocation = Path.Combine(AppDataLocation, "saves");

    public string CurrentSaveLocation => Path.Combine(SavesLocation, SaveName);

    public Settings(int seed, string saveName, float aspectRatio)
    {
        Seed = seed;
        SaveName = saveName;
        AspectRatio = aspectRatio;
    }
}