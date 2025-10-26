using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using Orchidic.Service;
using Orchidic.Utils;
using ReactiveUI;

namespace Orchidic.ViewModels;

public class PlayingPageViewModel : ViewModelBase, IDisposable
{
    private IPlayerService _playerService;
    public Bitmap Bitmap { get; }

    public Task<Bitmap> BlurredBitmap { get; }

    private readonly DispatcherTimer updateTimer = new() { Interval = TimeSpan.FromSeconds(0.2) };

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

    public PlayingPageViewModel(IPlayerService playerService)
    {
        _playerService = playerService;

        var uri = new Uri("avares://Orchidic/Assets/test4.png");
        using var stream = AssetLoader.Open(uri);
        Bitmap = new Bitmap(stream);
        BlurredBitmap = CreateBlurredBitmapAsync(Bitmap, 400);
        CurrentTime = TimeSpan.Zero;
        TotalTime = TimeSpan.Zero;
        Progress = 0;
        updateTimer.Tick += (s, e) =>
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

        _playerService.LoadFile(@"D:\Music\やなぎなぎ - over  and over.mp3");
        _playerService.Play();
    }

    private static async Task<Bitmap> CreateBlurredBitmapAsync(Bitmap source, float blurRadius)
    {
        return await Task.Run(() => source.CreateBlurredBitmap(blurRadius));
    }

    public void Dispose()
    {
        Bitmap.Dispose();
        BlurredBitmap.Dispose();
        updateTimer.Stop();
    }
}