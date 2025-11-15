using Orchidic.Utils;

namespace Orchidic.Models;

public class AudioFile : ReactiveObject
{
    public string Path { get; }

    public string Name => System.IO.Path.GetFileNameWithoutExtension(Path);

    private BitmapSource _thumbnails;
    
    public BitmapSource Thumbnails
    {
        get => _thumbnails;
        private set => this.RaiseAndSetIfChanged(ref _thumbnails, value);
    }
    

    public AudioFile(string? path = null)
    {
        Path = path ?? "";
        _thumbnails = AudioFileUtils.GetDefaultCover();
        Task.Run(() => { Thumbnails = AudioFileUtils.GetThumbnailsCoverFromAudio(Path); });
    }
}