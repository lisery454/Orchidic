using Orchidic.Services.Interfaces;
using Orchidic.Utils;


namespace Orchidic.ViewModels;

public class QueuePageViewModel : ViewModelBase
{
    public IPlayerService PlayerService { get; }
    public IAudioQueueService AudioQueueService { get; }

    public QueuePageViewModel(IPlayerService playerService, IAudioQueueService audioQueueService)
    {
        PlayerService = playerService;
        AudioQueueService = audioQueueService;
    }
}