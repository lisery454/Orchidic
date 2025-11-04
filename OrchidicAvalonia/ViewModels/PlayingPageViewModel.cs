using System;
using System.Windows.Input;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using Orchidic.Services.Interfaces;
using Orchidic.Utils;
using ReactiveUI;

namespace Orchidic.ViewModels;

public class PlayingPageViewModel : ViewModelBase, IDisposable
{
    private readonly IPlayerService _playerService;

    // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
    private readonly IFileInfoService _fileInfoService;
    private Bitmap _cover;

    public Bitmap Cover
    {
        get => _cover;
        set
        {
            if (ReferenceEquals(value, _cover))
                return;
            var old = _cover;
            this.RaiseAndSetIfChanged(ref _cover, value);
            old.Dispose();
        }
    }

    private string? currAudioPath { get; set; }

    private readonly DispatcherTimer updateTimer = new() { Interval = TimeSpan.FromSeconds(0.2) };
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
        set => this.RaiseAndSetIfChanged(ref _title, value);
    }

    private double _volume;

    public double Volume
    {
        get => _volume;
        set
        {
            var newValue = Math.Clamp(value, 0, 1);
            _playerService.SetVolume((float)newValue);
            this.RaiseAndSetIfChanged(ref _volume, newValue);
        }
    }

    private TimeSpan _totalTime;

    public TimeSpan TotalTime
    {
        get => _totalTime;
        set => this.RaiseAndSetIfChanged(ref _totalTime, value);
    }

    private TimeSpan _currentTime;

    public TimeSpan CurrentTime
    {
        get => _currentTime;
        set => this.RaiseAndSetIfChanged(ref _currentTime, value);
    }

    private double _progress;

    public double Progress
    {
        get => _progress;
        set
        {
            RawUpdateProgress(value);
            var newCurrentTime = new TimeSpan(0, 0, 0, (int)(_progress * TotalTime.TotalSeconds));
            _playerService.Seek(newCurrentTime);
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

    public PlayingPageViewModel(IPlayerService playerService, IFileInfoService fileInfoService)
    {
        _playerService = playerService;
        _fileInfoService = fileInfoService;
        currAudioPath = null;
        _volume = _playerService.GetVolume();
        _cover = _fileInfoService.GetDefaultCover();
        CurrentTime = playerService.GetCurrentTime();
        TotalTime = playerService.GetTotalTime();
        _title = _fileInfoService.GetDefaultTitle();
        _audioOperationCommandEnable = false;
        RawUpdateProgress(CurrentTime.TotalSeconds / TotalTime.TotalSeconds);
        updateTimer.Tick += (_, _) => { UpdateCurrAudioPath(); };
        updateTimer.Start();
        PlayOrPauseCommand = ReactiveCommand.Create(() =>
        {
            AudioOperationCommandEnable = false;
            if (_playerService.IsPlaying())
                _playerService.Pause();
            else
                _playerService.Play();
            UpdateCurrAudioPath();
        }, this.WhenAnyValue(x => x.AudioOperationCommandEnable));
        NextAudioCommand = ReactiveCommand.Create(() =>
        {
            AudioOperationCommandEnable = false;
            _playerService.Next();
            UpdateCurrAudioPath();
        }, this.WhenAnyValue(x => x.AudioOperationCommandEnable));
        PrevAudioCommand = ReactiveCommand.Create(() =>
        {
            AudioOperationCommandEnable = false;
            _playerService.Prev();
            UpdateCurrAudioPath();
        }, this.WhenAnyValue(x => x.AudioOperationCommandEnable));

        UpdateCurrAudioPath();
    }

    private void UpdateCurrAudioPath()
    {
        var path = _playerService.GetCurrentAudioFile()?.FilePath;
        
        AudioOperationCommandEnable = path != null;
        
        if (currAudioPath != path)
        {
            IsCancelDragRequested = true;
            Cover = path != null ? _fileInfoService.GetCoverFromAudio(path) : _fileInfoService.GetDefaultCover();
            Title = path != null ? _fileInfoService.GetTitleFromAudio(path) : _fileInfoService.GetDefaultTitle();
            currAudioPath = path;
        }
        
        CurrentTime = _playerService.GetCurrentTime();
        TotalTime = _playerService.GetTotalTime();
        RawUpdateProgress(CurrentTime.TotalSeconds / TotalTime.TotalSeconds);
    }

    public void Dispose()
    {
        Cover.Dispose();
        updateTimer.Stop();
    }
}