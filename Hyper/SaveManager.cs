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

    public static void DeleteSaves(string[] saveNames)
    {
        foreach (var saveName in saveNames)
        {
            Directory.Delete(Path.Combine(Settings.SavesLocation, saveName), recursive: true);
        }
    }

    public static void DeleteAllSaves()
    {
        DirectoryInfo di = new DirectoryInfo(Settings.SavesLocation);
        foreach (FileInfo file in di.GetFiles())
        {
            file.Delete();
        }
        foreach (DirectoryInfo dir in di.GetDirectories())
        {
            dir.Delete(recursive: true);
        }
    }
}