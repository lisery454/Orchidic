namespace Orchidic.Models;

public class AudioListSetting
{
    public string Name { get; set; }
    public string Path { get; set; }

    // ReSharper disable once UnusedMember.Global
    public AudioListSetting()
    {
        Name = "";
        Path = "";
    } // 必须有

    // ReSharper disable once ConvertToPrimaryConstructor
    public AudioListSetting(string name, string path)
    {
        Name = name;
        Path = path;
    }
}

public class Setting
{
    public float Volume { get; set; } = 0.3f;

    public List<string> QueuePaths { get; set; } = [];

    public string? CurrentAudioPath { get; set; }

    public AudioListSetting[] AudioListInfo { get; set; } = [];
}