using System.Text.Json;
using System.Text.Json.Serialization;

namespace Common;

/// <summary>
/// Manages and persists game settings including save data, geometry types, rendering, and time tracking.
/// </summary>
public class Settings
{
    private static readonly string AppDataLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Hyper");

    public static readonly string SavesLocation = Path.Combine(AppDataLocation, "saves");

    public string SaveName { get; private set; }

    public string CurrentSaveLocation => Path.Combine(SavesLocation, SaveName);

    public const string SaveFileName = "settings.json";

    public int Seed { get; private set; }

    public SelectedGeometryType SelectedGeometryType { get; private set; }

    public int RenderDistance { get; set; } = 3;

    /// <summary>
    /// Time elapsed since the last day start in seconds
    /// </summary>
    public float TimeInSeconds { get; set; } = 60 * 1;

    [JsonIgnore]
    public float AspectRatio { get; set; }

    /// <summary>
    /// Creates an instance of game settings with provided initial values.
    /// </summary>
    /// <param name="seed">Seed for procedural generation.</param>
    /// <param name="saveName">Name of the save.</param>
    /// <param name="aspectRatio">Aspect ratio of the game window.</param>
    /// <param name="selectedGeometryType">Type of selected geometry.</param>
    public Settings(int seed, string saveName, float aspectRatio, SelectedGeometryType selectedGeometryType)
    {
        Seed = seed;
        SaveName = saveName;
        AspectRatio = aspectRatio;
        SelectedGeometryType = selectedGeometryType;

        if (!Directory.Exists(CurrentSaveLocation))
            Directory.CreateDirectory(CurrentSaveLocation);
    }

    /// <summary>
    /// Checks if a save exists.
    /// </summary>
    /// <param name="saveName">Name of the save.</param>
    /// <returns>True if the save exists, otherwise false.</returns>
    public static bool SaveExists(string saveName)
    {
        return Directory.Exists(Path.Combine(SavesLocation, saveName));
    }

    /// <summary>
    /// Loads settings from the specified save.
    /// </summary>
    /// <param name="saveName">Name of the save.</param>
    /// <returns>Loaded settings.</returns>
    public static Settings Load(string saveName)
    {
        var json = File.ReadAllText(Path.Combine(SavesLocation, saveName, SaveFileName));
        var retrievedSettings = JsonSerializer.Deserialize<Settings>(json)!;

        return retrievedSettings;
    }

    /// <summary>
    /// Saves the current settings.
    /// </summary>
    public void Save()
    {
        var json = JsonSerializer.Serialize(this);
        File.WriteAllText(Path.Combine(CurrentSaveLocation, SaveFileName), json);
    }
}