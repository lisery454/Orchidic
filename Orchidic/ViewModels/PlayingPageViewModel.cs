using Orchidic.Services.Interfaces;
using Orchidic.Utils;


namespace Orchidic.ViewModels;

public class PlayingPageViewModel : ViewModelBase, IDisposable
{
    private readonly IPlayerService _playerService;
    private readonly IFileInfoService _fileInfoService;

    #region Property

    private BitmapSource _cover;

    public BitmapSource Cover
    {
        get => _cover;
        private set => this.RaiseAndSetIfChanged(ref _cover, value);
    }

    private BitmapSource? _blurCover;

    public BitmapSource? BlurCover
    {
        get => _blurCover;
        private set => this.RaiseAndSetIfChanged(ref _blurCover, value);
    }

    private string? CurrAudioPath { get; set; }

    private readonly DispatcherTimer _updateTimer = new() { Interval = TimeSpan.FromSeconds(1) };
    public ICommand NextAudioCommand { get; set; }
    public ICommand PrevAudioCommand { get; set; }
    public ICommand PlayOrPauseCommand { get; set; }

    private bool _audioOperationCommandEnable;

    private bool AudioOperationCommandEnable
    {
        get => _audioOperationCommandEnable;
        set => this.RaiseAndSetIfChanged(ref _audioOperationCommandEnable, value);
    }

    private string _title;

    public string Title
    {
        get => _title;
        private set => this.RaiseAndSetIfChanged(ref _title, value);
    }

    private double _volume;

    public double Volume
    {
        get => _volume;
        set
        {
            var newValue = Math.Clamp(value, 0, 1);
            _playerService.SetVolumeAsync((float)newValue);
            this.RaiseAndSetIfChanged(ref _volume, newValue);
        }
    }

    private TimeSpan _totalTime;

    public TimeSpan TotalTime
    {
        get => _totalTime;
        private set => this.RaiseAndSetIfChanged(ref _totalTime, value);
    }

    private TimeSpan _currentTime;

    public TimeSpan CurrentTime
    {
        get => _currentTime;
        private set => this.RaiseAndSetIfChanged(ref _currentTime, value);
    }

    private double _progress;

    public double Progress
    {
        get => _progress;
        set
        {
            RawUpdateProgress(value);
            var newCurrentTime = new TimeSpan(0, 0, 0, (int)(_progress * TotalTime.TotalSeconds));
            _playerService.SeekAsync(newCurrentTime);
            CurrentTime = newCurrentTime;
        }
    }

    private void RawUpdateProgress(double value)
    {
        this.RaiseAndSetIfChanged(ref _progress, Math.Clamp(value, 0, 1), nameof(Progress));
    }

    private bool _isCancelDragRequested;

    public bool IsCancelDragRequested
    {
        get => _isCancelDragRequested;
        set => this.RaiseAndSetIfChanged(ref _isCancelDragRequested, value);
    }

    #endregion

    private async Task LoadAsync()
    {
        Volume = await _playerService.GetVolumeAsync();
        CurrentTime = await _playerService.GetCurrentTimeAsync();
        TotalTime = await _playerService.GetTotalTimeAsync();
    }

    public PlayingPageViewModel(IPlayerService playerService, IFileInfoService fileInfoService)
    {
        _playerService = playerService;
        _fileInfoService = fileInfoService;
        CurrAudioPath = null;
        _volume = 0;
        _cover = _fileInfoService.GetDefaultCover();
        _currentTime = new TimeSpan(0);
        _totalTime = new TimeSpan(0);
        _ = LoadAsync();
        _title = _fileInfoService.GetDefaultTitle();
        _audioOperationCommandEnable = false;
        RawUpdateProgress(CurrentTime.TotalSeconds / TotalTime.TotalSeconds);
        _updateTimer.Tick += (_, _) => { _ = UpdateCurrAudioPathAsync(); };
        _updateTimer.Start();
        PlayOrPauseCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            AudioOperationCommandEnable = false;
            if (await _playerService.IsPlayingAsync())
                await _playerService.PauseAsync();
            else
                await _playerService.PlayAsync();
            await Application.Current.Dispatcher.InvokeAsync(async () => await UpdateCurrAudioPathAsync());
        }, this.WhenAnyValue(x => x.AudioOperationCommandEnable));
        NextAudioCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            AudioOperationCommandEnable = false;
            await _playerService.NextAsync();
            await Application.Current.Dispatcher.InvokeAsync(async () => await UpdateCurrAudioPathAsync());
        }, this.WhenAnyValue(x => x.AudioOperationCommandEnable));
        PrevAudioCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            AudioOperationCommandEnable = false;
            await _playerService.PrevAsync();
            await Application.Current.Dispatcher.InvokeAsync(async () => await UpdateCurrAudioPathAsync());
        }, this.WhenAnyValue(x => x.AudioOperationCommandEnable));

        _ = Application.Current.Dispatcher.InvokeAsync(async () => await UpdateCurrAudioPathAsync());
    }

    private async Task UpdateCurrAudioPathAsync()
    {
        var path = _playerService.GetCurrentAudioFile()?.FilePath;

        AudioOperationCommandEnable = path != null;

        if (CurrAudioPath != path)
        {
            IsCancelDragRequested = true;
            Cover = path != null ? _fileInfoService.GetCoverFromAudio(path) : _fileInfoService.GetDefaultCover();

            Title = path != null ? _fileInfoService.GetTitleFromAudio(path) : _fileInfoService.GetDefaultTitle();
            CurrAudioPath = path;
            _ = UpdateBlurCover();
        }

        CurrentTime = await _playerService.GetCurrentTimeAsync();
        TotalTime = await _playerService.GetTotalTimeAsync();
        RawUpdateProgress(CurrentTime.TotalSeconds / TotalTime.TotalSeconds);
    }

    private async Task UpdateBlurCover()
    {
        BlurCover = await _fileInfoService.GetBlurCoverFromCover(Cover, CurrAudioPath);
    }

    public void Dispose()
    {
        _updateTimer.Stop();
    }
}