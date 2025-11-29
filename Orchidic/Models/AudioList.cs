using Orchidic.Utils;

namespace Orchidic.Models;

public class AudioList : ReactiveObject
{
    public ObservableCollection<string> AudioFilePaths { get; }

    public string _directoryPath = string.Empty;

    public string DirectoryPath
    {
        get => _directoryPath;
        set => this.RaiseAndSetIfChanged(ref _directoryPath, value);
    }

    private string _name = string.Empty;

    public string Name
    {
        get => _name;
        private set => this.RaiseAndSetIfChanged(ref _name, value);
    }
    
    string[] audioExtensions =
    [
        ".mp3", ".wav", ".flac", ".aac", ".ogg", ".m4a", ".wma", ".aiff", ".alac"
    ];

    public AudioList(string name, string directoryPath)
    {
        Name = name;
        DirectoryPath = directoryPath;
        
        var audioFiles = Directory
            .EnumerateFiles(DirectoryPath, "*", SearchOption.AllDirectories)
            .Where(file => audioExtensions.Contains(Path.GetExtension(file).ToLower()))
            .ToList();
        
        
        AudioFilePaths = new ObservableCollection<string>(audioFiles);

        _thumbnails = null;
        _lazyThumbnails = new Lazy<Task<BitmapSource>>(async () =>
        {
            var firstPath = AudioFilePaths.First();
            var bmp = await Task.Run(() => AudioFileUtils.GetThumbnailsCoverFromAudio(firstPath));
            App.Current.Dispatcher.Invoke(() => Thumbnails = bmp);
            return bmp;
        });
    }

    private BitmapSource? _thumbnails;

    private readonly Lazy<Task<BitmapSource>> _lazyThumbnails;

    public BitmapSource? Thumbnails
    {
        get
        {
            // 触发加载
            if (!_lazyThumbnails.Value.IsCompleted)
            {
                _ = _lazyThumbnails.Value; // 启动 Task，但不阻塞
            }

            return _thumbnails;
        }
        private set => this.RaiseAndSetIfChanged(ref _thumbnails, value);
    }
}