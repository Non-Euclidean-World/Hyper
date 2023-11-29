using System.Text.Json;
using System.Text.Json.Serialization;

namespace Common;

public class Settings
{
    private static readonly string AppDataLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Hyper");

    public static readonly string SavesLocation = Path.Combine(AppDataLocation, "saves");

    public string SaveName { get; private set; }

    public string CurrentSaveLocation => Path.Combine(SavesLocation, SaveName);

    public const string SaveFileName = "settings.json";

    public int Seed { get; private set; }

    public GeometryType GeometryType { get; private set; }

    public int RenderDistance { get; set; } = 2;

    /// <summary>
    /// Time elapsed since the last day start in seconds
    /// </summary>
    public float TimeInSeconds { get; set; } = 0;

    [JsonIgnore]
    public float AspectRatio { get; set; }

    public Settings(int seed, string saveName, float aspectRatio, GeometryType geometryType)
    {
        Seed = seed;
        SaveName = saveName;
        AspectRatio = aspectRatio;
        GeometryType = geometryType;

        if (!Directory.Exists(CurrentSaveLocation))
            Directory.CreateDirectory(CurrentSaveLocation);
    }

    public static bool SaveExists(string saveName)
    {
        return Directory.Exists(Path.Combine(SavesLocation, saveName));
    }

    public static Settings Load(string saveName)
    {
        var json = File.ReadAllText(Path.Combine(SavesLocation, saveName, SaveFileName));
        var retrievedSettings = JsonSerializer.Deserialize<Settings>(json)!;

        return retrievedSettings;
    }

    public void Save()
    {
        var json = JsonSerializer.Serialize(this);
        File.WriteAllText(Path.Combine(CurrentSaveLocation, SaveFileName), json);
    }
}