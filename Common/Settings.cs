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

    private string SettingsLocation => Path.Combine(CurrentSaveLocation, "settings.txt");

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

        File.WriteAllLines(SettingsLocation, settings.Select(s => $"{s.Key}={s.Value}"));
    }

    private void Load()
    {
        var settings = File.ReadAllLines(SettingsLocation);
        foreach (var setting in settings)
        {
            var split = setting.Split('=');
            switch (split[0])
            {
                case "seed":
                    Seed = int.Parse(split[1]);
                    break;
                case "saveName":
                    SaveName = split[1];
                    break;
            }
        }
    }
}