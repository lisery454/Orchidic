using Orchidic.Models;
using Orchidic.Services.Interfaces;
using Orchidic.Utils;
using Orchidic.Utils.SettingManager;

namespace Orchidic.Services;

public class AudioQueueService : ReactiveObject, IAudioQueueService
{
    public AudioQueue AudioQueue { get; }

    private ISettingManager _settingManager;

    public AudioQueueService(ISettingManager settingManager)
    {
        _settingManager = settingManager;
        AudioQueue = new AudioQueue(_settingManager.CurrentSetting.QueuePaths.Select(x => new AudioFile(x)).ToList());

        UpdateCover(AudioQueue.CurrentAudioFile);

        AudioQueue.TrySetCurrentAudioFile(settingManager.CurrentSetting.CurrentAudioPath);


        AudioQueue.WhenAnyValue(x => x.CurrentAudioFile).Subscribe(currentAudioFile =>
        {
            UpdateCover(currentAudioFile);

            _settingManager.CurrentSetting.CurrentAudioPath = currentAudioFile?.Path;
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
        _settingManager.CurrentSetting.QueuePaths = AudioQueue.AudioFiles.Select(x => x.Path).ToList();
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