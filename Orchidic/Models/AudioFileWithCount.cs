namespace Orchidic.Models;

public class AudioFileWithCount : ReactiveObject
{
    public AudioFile File { get; }
    
    private int _count;
    public int Count
    {
        get => _count;
        set => this.RaiseAndSetIfChanged(ref _count, value);
    }

    public AudioFileWithCount(AudioFile file, int count)
    {
        File = file;
        _count = count;
    }
}