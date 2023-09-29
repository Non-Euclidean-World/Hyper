using Common;

namespace Hyper;

public static class SaveManager
{
    public static List<string> GetSaves()
    {
        return Directory.GetDirectories(Settings.SavesLocation)
            .Select(f => new FileInfo(f))
            .OrderByDescending(f => f.LastWriteTime)
            .Select(f => f.Name)
            .ToList();
    }

    public static void DeleteSave(string saveName)
    {
        Directory.Delete(Path.Combine(Settings.SavesLocation, saveName), true);
    }
}