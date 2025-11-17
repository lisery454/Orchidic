using Orchidic.Models;
using Orchidic.Services.Interfaces;
using Orchidic.Utils;

namespace Orchidic.Services;

public class AudioQueueService : ReactiveObject, IAudioQueueService
{
    public AudioQueue AudioQueue { get; } = new();

    public AudioQueueService()
    {
        _currentCover = AudioQueue.CurrentAudioFile != null
            ? AudioFileUtils.GetCoverFromAudio(AudioQueue.CurrentAudioFile.Path)
            : AudioFileUtils.GetDefaultCover();
        Task.Run(async () =>
        {
            var image = await AudioFileUtils.GetBlurCoverFromCover(CurrentCover, AudioQueue.CurrentAudioFile?.Path);
            App.Current.Dispatcher.Invoke(() => { CurrentBlurCover = image; });
        });

        AudioQueue.WhenAnyValue(x => x.CurrentAudioFile).Subscribe(currentAudioFile =>
        {
            Task.Run(async () =>
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    CurrentCover = currentAudioFile != null
                        ? AudioFileUtils.GetCoverFromAudio(currentAudioFile.Path)
                        : AudioFileUtils.GetDefaultCover();
                });
                var image = await AudioFileUtils.GetBlurCoverFromCover(CurrentCover, currentAudioFile?.Path);
                App.Current.Dispatcher.Invoke(() => { CurrentBlurCover = image; });
            });
        });
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
}