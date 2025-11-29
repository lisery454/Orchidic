using Orchidic.Models;
using Orchidic.Services.Interfaces;
using Orchidic.Utils;

namespace Orchidic.Services;

public class AudioQueueService : ReactiveObject, IAudioQueueService
{
    public AudioQueue AudioQueue { get; }

    private readonly ISettingService _settingService;


    public AudioQueueService(ISettingService settingService)
    {
        _settingService = settingService;
        AudioQueue = new AudioQueue(_settingService.CurrentSetting.QueuePaths.Select(x => new AudioFile(x)).ToList(),
            _settingService.CurrentSetting.PlaybackOrder, _settingService.CurrentSetting.IsSingleLoop);

        UpdateCover(AudioQueue.CurrentAudioFile);

        AudioQueue.TrySetCurrentAudioFile(settingService.CurrentSetting.CurrentAudioPath);


        AudioQueue.WhenAnyValue(x => x.CurrentAudioFile).Subscribe(currentAudioFile =>
        {
            UpdateCover(currentAudioFile);
            _settingService.CurrentSetting.CurrentAudioPath = currentAudioFile?.Path;
        });

        AudioQueue.WhenAnyValue(x => x.PlaybackOrder).Subscribe(order =>
        {
            _settingService.CurrentSetting.PlaybackOrder = order;
        });

        AudioQueue.WhenAnyValue(x => x.IsSingleLoop).Subscribe(isSingleLoop =>
        {
            _settingService.CurrentSetting.IsSingleLoop = isSingleLoop;
        });

        AudioQueue.AudioFiles.CollectionChanged += AudioFilesOnCollectionChanged;
    }

    private void UpdateCover(AudioFile? currentAudioFile)
    {
        Task.Run(async () =>
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                CurrentCover =
                    AudioFileUtils.GetCoverFromAudio(currentAudioFile?.Path);
            });
            var image = await AudioFileUtils.GetBlurCoverFromCover(CurrentCover, currentAudioFile?.Path);
            App.Current.Dispatcher.Invoke(() => { CurrentBlurCover = image; });
        });
    }

    private void AudioFilesOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        _settingService.CurrentSetting.QueuePaths = AudioQueue.AudioFiles.Select(x => x.Path).ToList();
    }

    private BitmapSource? _currentCover;

    public BitmapSource? CurrentCover
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
}