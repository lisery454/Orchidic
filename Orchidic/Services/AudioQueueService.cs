using Orchidic.Models;
using Orchidic.Services.Interfaces;
using Orchidic.Utils;

namespace Orchidic.Services;

public class AudioQueueService : ReactiveObject, IAudioQueueService
{
    private readonly AudioQueue _audioQueue = new();
    public AudioQueueService()
    {
        _title = CurrentAudioFile != null
            ? AudioFileUtils.GetTitleFromAudio(CurrentAudioFile.Path)
            : AudioFileUtils.GetDefaultTitle();
        _currentCover = CurrentAudioFile != null
            ? AudioFileUtils.GetCoverFromAudio(CurrentAudioFile.Path)
            : AudioFileUtils.GetDefaultCover();
        Task.Run(async () =>
        {
            var image = await AudioFileUtils.GetBlurCoverFromCover(CurrentCover, CurrentAudioFile?.Path);
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
    
    public ObservableCollection<AudioFile> AudioFiles => _audioQueue.AudioFiles;

    private int _currentIndex;

    public int CurrentIndex
    {
        get => _audioQueue.CurrentIndex;
        set
        {
            this.RaiseAndSetIfChanged(ref _currentIndex, value);
            _audioQueue.CurrentIndex = value;
            Title = CurrentAudioFile != null
                ? AudioFileUtils.GetTitleFromAudio(CurrentAudioFile.Path)
                : AudioFileUtils.GetDefaultTitle();
            CurrentCover = CurrentAudioFile != null
                ? AudioFileUtils.GetCoverFromAudio(CurrentAudioFile.Path)
                : AudioFileUtils.GetDefaultCover();
            Task.Run(async () =>
            {
                var image = await AudioFileUtils.GetBlurCoverFromCover(CurrentCover, CurrentAudioFile?.Path);
                App.Current.Dispatcher.Invoke(() => { CurrentBlurCover = image; });
            });
        }
    }

    public AudioFile? CurrentAudioFile => _audioQueue.CurrentAudioFile;
}