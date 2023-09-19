using System.Text.Json;

namespace Common;

public class Settings
{
    private static readonly Lazy<Settings> _instance = new(() => new Settings());

    public static Settings Instance => _instance.Value;

    public int Seed { get; private set; } = 0;

    public string SaveName { get; private set; } = "default";

    private static readonly string AppDataLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Hyper");

    public static readonly string SavesLocation = Path.Combine(AppDataLocation, "saves");

    public string CurrentSaveLocation => Path.Combine(SavesLocation, SaveName);

    private string SettingsLocation => Path.Combine(CurrentSaveLocation, "settings.json");

    private Settings()
    {
        Directory.CreateDirectory(AppDataLocation);
        Directory.CreateDirectory(SavesLocation);
    }

    public void Initialize(string saveName)
    {
        SaveName = saveName;

        if (Directory.Exists(CurrentSaveLocation))
        {
            Load();
            return;
        }

        Seed = new Random().Next();
        Directory.CreateDirectory(CurrentSaveLocation);
    }

    public void Save()
    {
        var settings = new Dictionary<string, string>
        {
            {"seed", Seed.ToString()},
            {"saveName", SaveName}
        };

        File.WriteAllText(SettingsLocation, JsonSerializer.Serialize(settings));
    }

    private void Load()
    {
        var json = File.ReadAllText(SettingsLocation);
        var settings = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

        Seed = int.Parse(settings["seed"]);
        SaveName = settings["saveName"];
    }
}