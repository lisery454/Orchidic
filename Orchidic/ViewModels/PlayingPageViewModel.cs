using Orchidic.Services.Interfaces;
using Orchidic.Utils;


namespace Orchidic.ViewModels;

public class PlayingPageViewModel : ViewModelBase
{
    private readonly IPlayerService _playerService;
    private readonly IAudioQueueService _audioQueueService;

    #region Property

    public ICommand NextAudioCommand { get; }
    public ICommand PrevAudioCommand { get; }
    public ICommand PlayOrPauseCommand { get; }

    public BitmapSource CurrentCover => _audioQueueService.CurrentCover;
    public BitmapSource? CurrentBlurCover => _audioQueueService.CurrentBlurCover;
    
    public string Title => _audioQueueService.Title;

    private readonly ObservableAsPropertyHelper<float> _volume;

    public float Volume
    {
        get => _volume.Value;
        set => _playerService.Volume = value;
    }

    private readonly ObservableAsPropertyHelper<TimeSpan> _currentTime;
    public TimeSpan CurrentTime => _currentTime.Value;

    private readonly ObservableAsPropertyHelper<TimeSpan> _totalTime;
    public TimeSpan TotalTime => _totalTime.Value;


    public float Progress
    {
        get => _playerService.Progress;
        set => _playerService.Progress = value;
    }

    #endregion


    public PlayingPageViewModel(IPlayerService playerService, IAudioQueueService audioQueueService)
    {
        _playerService = playerService;
        _audioQueueService = audioQueueService;

        // CurrentTime 属性绑定
        _playerService.CurrentTimeObservable
            .ToProperty(this, x => x.CurrentTime, out _currentTime);

        // TotalTime 属性绑定
        _playerService.TotalTimeObservable
            .ToProperty(this, x => x.TotalTime, out _totalTime);

        // Progress 属性绑定
        this.WhenAnyValue(x => x._playerService.Progress)
            .Subscribe(_ => this.RaisePropertyChanged(nameof(Progress)));

        // Volume 属性绑定
        _playerService.VolumeObservable
            .ToProperty(this, x => x.Volume, out _volume);

        // Title 属性绑定
        this.WhenAnyValue(x => x._audioQueueService.Title)
            .Subscribe(_ => this.RaisePropertyChanged(nameof(Title)));
        
        // CurrentCover 属性绑定
        this.WhenAnyValue(x => x._audioQueueService.CurrentCover)
            .Subscribe(_ => this.RaisePropertyChanged(nameof(CurrentCover)));
        
        // CurrentBlurCover 属性绑定
        this.WhenAnyValue(x => x._audioQueueService.CurrentBlurCover)
            .Subscribe(_ => this.RaisePropertyChanged(nameof(CurrentBlurCover)));

        PlayOrPauseCommand = ReactiveCommand.Create(() =>
        {
            if (_playerService.IsPlaying)
                _playerService.Pause();
            else
                _playerService.Resume();
        });
        NextAudioCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            _audioQueueService.CurrentIndex += 1;
            var currentAudioFile = _audioQueueService.CurrentAudioFile;
            if (currentAudioFile != null)
            {
                await _playerService.PlayAsync(currentAudioFile);
            }
        });
        PrevAudioCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            _audioQueueService.CurrentIndex -= 1;
            var currentAudioFile = _audioQueueService.CurrentAudioFile;
            if (currentAudioFile != null)
            {
                await _playerService.PlayAsync(currentAudioFile);
            }
        });

        if (_audioQueueService.CurrentAudioFile != null)
            _ = _playerService.PlayAsync(_audioQueueService.CurrentAudioFile);
    }
}