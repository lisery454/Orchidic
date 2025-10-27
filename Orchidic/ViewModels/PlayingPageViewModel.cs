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
        set => this.RaiseAndSetIfChanged(ref _cover, value);
    }

    private readonly DispatcherTimer updateTimer = new() { Interval = TimeSpan.FromSeconds(0.2) };
    public ICommand NextAudioCommand { get; set; }
    public ICommand PrevAudioCommand { get; set; }
    public ICommand PlayOrPauseCommand { get; set; }
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

    public PlayingPageViewModel(IPlayerService playerService, IFileInfoService fileInfoService)
    {
        _playerService = playerService;
        _fileInfoService = fileInfoService;

        _cover = _fileInfoService.GetDefaultCover();
        CurrentTime = playerService.GetCurrentTime();
        TotalTime = playerService.GetTotalTime();
        RawUpdateProgress(CurrentTime.TotalSeconds / TotalTime.TotalSeconds);
        updateTimer.Tick += (_, _) =>
        {
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
            var path = _playerService.GetCurrentAudioFile()?.path;
            if (path != null)
                Cover = _fileInfoService.GetCoverFromAudio(path);
            CurrentTime = playerService.GetCurrentTime();
            TotalTime = playerService.GetTotalTime();
            RawUpdateProgress(CurrentTime.TotalSeconds / TotalTime.TotalSeconds);
        });
        PrevAudioCommand = ReactiveCommand.Create(() =>
        {
            _playerService.Prev();
            var path = _playerService.GetCurrentAudioFile()?.path;
            if (path != null)
                Cover = _fileInfoService.GetCoverFromAudio(path);
            CurrentTime = playerService.GetCurrentTime();
            TotalTime = playerService.GetTotalTime();
            RawUpdateProgress(CurrentTime.TotalSeconds / TotalTime.TotalSeconds);
        });

        var path = _playerService.GetCurrentAudioFile()?.path;
        if (path != null)
            Cover = _fileInfoService.GetCoverFromAudio(path);
    }

    public void Dispose()
    {
        Cover.Dispose();
        updateTimer.Stop();
    }
}