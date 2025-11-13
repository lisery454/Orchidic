using Orchidic.Models;
using Orchidic.Services.Interfaces;

namespace Orchidic.Services;

public class AudioQueueService : ReactiveObject, IAudioQueueService
{
    private readonly AudioQueue _audioQueue = new();
    private readonly IFileInfoService _fileInfoService;

    public AudioQueueService(IFileInfoService fileInfoService)
    {
        _fileInfoService = fileInfoService;
        _title = CurrentAudioFile != null
            ? _fileInfoService.GetTitleFromAudio(CurrentAudioFile.FilePath)
            : _fileInfoService.GetDefaultTitle();
        _currentCover = CurrentAudioFile != null
            ? _fileInfoService.GetCoverFromAudio(CurrentAudioFile.FilePath)
            : _fileInfoService.GetDefaultCover();
        Task.Run(async () =>
        {
            var image = await _fileInfoService.GetBlurCoverFromCover(CurrentCover, CurrentAudioFile?.FilePath);
            App.Current.Dispatcher.Invoke(() => { CurrentBlurCover = image; });
        });
    }

    private string _title;

    public string Title
    {
        get => _title;
        private set => this.RaiseAndSetIfChanged(ref _title, value);
    }

    private BitmapSource _currentCover;

    public BitmapSource CurrentCover
    {
        get => _currentCover;
        private set => this.RaiseAndSetIfChanged(ref _currentCover, value);
    }

    private BitmapSource? _currentBlurCover;

    public BitmapSource? CurrentBlurCover
    {
        get => _currentBlurCover;
        private set => this.RaiseAndSetIfChanged(ref _currentBlurCover, value);
    }

    public IObservable<IReadOnlyCollection<AudioFile>> AudioFilesObservable =>
        _audioQueue.AudioFilesObservable;

    private int _currentIndex;

    public int CurrentIndex
    {
        get => _audioQueue.CurrentIndex;
        set
        {
            this.RaiseAndSetIfChanged(ref _currentIndex, value);
            _audioQueue.CurrentIndex = value;
            Title = CurrentAudioFile != null
                ? _fileInfoService.GetTitleFromAudio(CurrentAudioFile.FilePath)
                : _fileInfoService.GetDefaultTitle();
            CurrentCover = CurrentAudioFile != null
                ? _fileInfoService.GetCoverFromAudio(CurrentAudioFile.FilePath)
                : _fileInfoService.GetDefaultCover();
            Task.Run(async () =>
            {
                var image = await _fileInfoService.GetBlurCoverFromCover(CurrentCover, CurrentAudioFile?.FilePath);
                App.Current.Dispatcher.Invoke(() => { CurrentBlurCover = image; });
            });
        }
    }

    public AudioFile? CurrentAudioFile => _audioQueue.CurrentAudioFile;
}