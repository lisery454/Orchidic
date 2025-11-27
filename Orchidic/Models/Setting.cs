namespace Orchidic.Models;

public class Setting
{
    public float Volume { get; set; } = 0.3f;

    public List<string> QueuePaths { get; set; } = [];

    public string? CurrentAudioPath { get; set; }

    public (string, string[])[] AudioListInfo { get; set; } = [];
}