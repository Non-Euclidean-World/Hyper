using System.Text.Json;

namespace Common;

public class Settings
{
    private static readonly string AppDataLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Hyper");

    public static readonly string SavesLocation = Path.Combine(AppDataLocation, "saves");

    public string SaveName { get; private set; }

    public string CurrentSaveLocation => Path.Combine(SavesLocation, SaveName);

    public const string SaveFileName = "settings.json";

    public int Seed { get; private set; }

    public float AspectRatio { get; private set; }

    public Settings(int seed, string saveName, float aspectRatio)
    {
        Seed = seed;
        SaveName = saveName;
        AspectRatio = aspectRatio;

        if (!Directory.Exists(CurrentSaveLocation)) Directory.CreateDirectory(CurrentSaveLocation);
    }

    public static Settings Load(string saveName, float aspectRatio)
    {
        if (!Directory.Exists(Path.Combine(SavesLocation, saveName)))
        {
            var rand = new Random();
            return new Settings(rand.Next(), saveName, aspectRatio);
        }
        var json = File.ReadAllText(Path.Combine(SavesLocation, saveName, SaveFileName));
        var settings = JsonSerializer.Deserialize<Settings>(json)!;
        settings.AspectRatio = aspectRatio;
        return settings;
    }

    public void Save()
    {
        var json = JsonSerializer.Serialize(this);
        File.WriteAllText(Path.Combine(CurrentSaveLocation, SaveFileName), json);
    }
}