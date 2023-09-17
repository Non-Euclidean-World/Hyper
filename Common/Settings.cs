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
    
    private Settings()
    {
        Directory.CreateDirectory(AppDataLocation);
        Directory.CreateDirectory(SavesLocation);
        Directory.CreateDirectory(CurrentSaveLocation);
    }

    public void Initialize(int seed, string saveName)
    {
        Seed = seed;
        SaveName = saveName;
        Directory.CreateDirectory(CurrentSaveLocation);
    }
}