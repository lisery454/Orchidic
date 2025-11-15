using Orchidic.Services.Interfaces;
using Orchidic.Utils;


namespace Orchidic.ViewModels;

public class PlayingPageViewModel : ViewModelBase
{
    #region Property

    public IPlayerService PlayerService { get; }
    public IAudioQueueService AudioQueueService { get; }

    public ICommand NextAudioCommand { get; }
    public ICommand PrevAudioCommand { get; }
    public ICommand PlayOrPauseCommand { get; }

    #endregion


    public PlayingPageViewModel(IPlayerService playerService, IAudioQueueService audioQueueService)
    {
        PlayerService = playerService;
        AudioQueueService = audioQueueService;

        PlayOrPauseCommand = ReactiveCommand.Create(() =>
        {
            if (PlayerService.IsPlaying)
                PlayerService.Pause();
            else
                PlayerService.Resume();
        });
        NextAudioCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            AudioQueueService.CurrentIndex += 1;
            var currentAudioFile = AudioQueueService.CurrentAudioFile;
            if (currentAudioFile != null)
            {
                await PlayerService.PlayAsync(currentAudioFile);
            }
        });
        PrevAudioCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            AudioQueueService.CurrentIndex -= 1;
            var currentAudioFile = AudioQueueService.CurrentAudioFile;
            if (currentAudioFile != null)
            {
                await PlayerService.PlayAsync(currentAudioFile);
            }
        });

        PlayerService.PlaybackEnded += async (_, _) =>
        {
            NextAudioCommand.Execute(null);
        };

        if (AudioQueueService.CurrentAudioFile != null)
            _ = PlayerService.PlayAsync(AudioQueueService.CurrentAudioFile);
    }
}