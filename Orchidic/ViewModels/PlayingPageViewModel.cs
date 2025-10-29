using System;
using System.Windows.Input;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using Orchidic.Service;
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
            if (value == _cover) return;
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
        RawUpdateProgress(CurrentTime.TotalSeconds / TotalTime.TotalSeconds);
        updateTimer.Tick += (_, _) =>
        {
            UpdateCurrAudioPath();
            CurrentTime = playerService.GetCurrentTime();
            TotalTime = playerService.GetTotalTime();
            RawUpdateProgress(CurrentTime.TotalSeconds / TotalTime.TotalSeconds);
        };
        updateTimer.Start();
        PlayOrPauseCommand = ReactiveCommand.Create(() =>
        {
            if (_playerService.IsPlaying())
            {
                _playerService.Pause();
            }
            else
            {
                _playerService.Play();
            }
        });
        NextAudioCommand = ReactiveCommand.Create(() =>
        {
            _playerService.Next();
            UpdateCurrAudioPath();
            CurrentTime = playerService.GetCurrentTime();
            TotalTime = playerService.GetTotalTime();
            RawUpdateProgress(CurrentTime.TotalSeconds / TotalTime.TotalSeconds);
        });
        PrevAudioCommand = ReactiveCommand.Create(() =>
        {
            _playerService.Prev();
            UpdateCurrAudioPath();
            CurrentTime = playerService.GetCurrentTime();
            TotalTime = playerService.GetTotalTime();
            RawUpdateProgress(CurrentTime.TotalSeconds / TotalTime.TotalSeconds);
        });

        UpdateCurrAudioPath();
    }

    private void UpdateCurrAudioPath()
    {
        var path = _playerService.GetCurrentAudioFile()?.path;
        if (currAudioPath == path) return;
        IsCancelDragRequested = true;
        Cover = path != null ? _fileInfoService.GetCoverFromAudio(path) : _fileInfoService.GetDefaultCover();
        Title = path != null ? _fileInfoService.GetTitleFromAudio(path) : _fileInfoService.GetDefaultTitle();
        currAudioPath = path;
    }

    public void Dispose()
    {
        Cover.Dispose();
        updateTimer.Stop();
    }
}