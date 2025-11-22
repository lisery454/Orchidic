using Orchidic.Utils;

namespace Orchidic.Models;

public class AudioFile : ReactiveObject
{
    public string Path { get; }

    public string Name => System.IO.Path.GetFileNameWithoutExtension(Path);

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

    public AudioFile(string? path = null)
    {
        Path = path ?? "";
        _thumbnails = null;
        _lazyThumbnails = new Lazy<Task<BitmapSource>>(async () =>
        {
            var bmp = await Task.Run(() => AudioFileUtils.GetThumbnailsCoverFromAudio(Path));
            // 异步返回后更新 UI 属性
            // 注意：必须在 UI 线程设置
            App.Current.Dispatcher.Invoke(() => Thumbnails = bmp);
            return bmp;
        });
    }
}