using Orchidic.Models;
using Orchidic.Services.Interfaces;
using Orchidic.Utils;


namespace Orchidic.ViewModels;

public class QueuePageViewModel : ViewModelBase
{
    private IPlayerService PlayerService { get; }
    public IAudioQueueService AudioQueueService { get; }

    public ICommand PlayCommand { get; }

    public QueuePageViewModel(IPlayerService playerService, IAudioQueueService audioQueueService)
    {
        PlayerService = playerService;
        AudioQueueService = audioQueueService;

        PlayCommand = ReactiveCommand.CreateFromTask<AudioFile>(async file =>
        {
            if (PlayerService.CurrentAudioFile == file)
            {
                if (!PlayerService.IsPlaying)
                {
                    PlayerService.Resume();
                }
            }
            else
            {
                AudioQueueService.AudioQueue.TrySetCurrentAudioFile(file);
                await PlayerService.PlayAsync(file);
            }
        });
    }
}

public class AudioFileEqualConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        var audioFile0 = values[0] as AudioFile;
        var audioFile1 = values[1] as AudioFile;
        return audioFile0 == audioFile1;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        return [];
    }
}