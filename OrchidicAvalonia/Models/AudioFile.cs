namespace Orchidic.Models;

public class AudioFile(string? path)
{
    public string FilePath => path ?? "";
}