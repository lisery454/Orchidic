namespace Orchidic.Utils;

public static class ProgramConstants
{
    public static string AppName => "Orchidic";

    public static string ApplicationPath
    {
        get
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                AppName);
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            return path;
        }
    }

    public static string AppDataLocalPath
    {
        get
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                AppName);
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            return path;
        }
    }

    public static string AudioCoverCacheDirPath => Path.Combine(AppDataLocalPath, "CoverCache");

    public static string SettingPath => Path.Combine(ApplicationPath, "setting.yaml");

    public static string LogPath => Path.Combine(ApplicationPath, "log.txt");
}