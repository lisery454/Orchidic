using Orchidic.Services.Interfaces;
using Orchidic.Utils;


namespace Orchidic.ViewModels;

public class PlayingPageViewModel : ViewModelBase
{
    #region Property

    public IPlayerService PlayerService { get; }
    public IAudioQueueService AudioQueueService { get; }
    public IGlobalService GlobalService { get; }

    public ICommand NextAudioCommand { get; }
    public ICommand PrevAudioCommand { get; }
    public ICommand PlayOrPauseCommand { get; }

    public ICommand SetZenModeCommand { get; }

    #endregion


    public PlayingPageViewModel(IPlayerService playerService, IAudioQueueService audioQueueService,
        IGlobalService globalService)
    {
        PlayerService = playerService;
        AudioQueueService = audioQueueService;
        GlobalService = globalService;

        var canPlay = AudioQueueService.AudioQueue
            .WhenAnyValue(x => x.CurrentAudioFile)
            .Select(file => file != null);

        PlayOrPauseCommand = ReactiveCommand.Create(() =>
        {
            if (PlayerService.IsPlaying)
                PlayerService.Pause();
            else
                PlayerService.Resume();
        }, canPlay);
        NextAudioCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            AudioQueueService.AudioQueue.CurrentIndex += 1;
            var currentAudioFile = AudioQueueService.AudioQueue.CurrentAudioFile;
            if (currentAudioFile != null)
            {
                await PlayerService.PlayAsync(currentAudioFile);
            }
        }, canPlay);
        PrevAudioCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            AudioQueueService.AudioQueue.CurrentIndex -= 1;
            var currentAudioFile = AudioQueueService.AudioQueue.CurrentAudioFile;
            if (currentAudioFile != null)
            {
                AudioQueueService.AudioQueue.TrySetCurrentAudioFile(currentAudioFile);
                await PlayerService.PlayAsync(currentAudioFile);
            }
        }, canPlay);
        SetZenModeCommand = ReactiveCommand.Create(() => { globalService.IsZenMode = !globalService.IsZenMode; });

        PlayerService.PlaybackEnded += (_, _) => { NextAudioCommand.Execute(null); };

        if (AudioQueueService.AudioQueue.CurrentAudioFile != null)
            _ = PlayerService.PlayAsync(AudioQueueService.AudioQueue.CurrentAudioFile);
    }
}