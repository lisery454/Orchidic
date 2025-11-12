using Orchidic.Models;
using Orchidic.Services.Interfaces;
using Orchidic.Utils.SettingManager;

namespace Orchidic.Services;

public class PlayerService : IPlayerService, IDisposable
{
    private readonly WaveOutEvent _device;
    private AudioFileReader? _currentAudioFileReader;
    private readonly AudioQueue _audioQueue;
    private readonly object _deviceLock = new(); // 保护 WaveOutEvent / AudioFileReader
    private bool _isPlaying;
    private readonly ISettingManager _settingManager;

    // 异步队列，保证方法按顺序执行
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    private AudioFileReader? CurrentAudioFileReader
    {
        get => _currentAudioFileReader;
        set
        {
            if (value == _currentAudioFileReader) return;
            _currentAudioFileReader?.Dispose();
            _currentAudioFileReader = value;
        }
    }

    public PlayerService(ISettingManager settingManager)
    {
        _settingManager = settingManager;
        _audioQueue = new AudioQueue();

        _device = new WaveOutEvent();
        _device.Volume = _settingManager.CurrentSetting.Volume;

        _device.PlaybackStopped += OnPlaybackStopped;
        _isPlaying = false;

        LoadFile(_audioQueue.CurrentAudioFile);
    }

    private async void OnPlaybackStopped(object? sender, StoppedEventArgs e)
    {
        if ((await GetTotalTimeAsync() - await GetCurrentTimeAsync()).Duration() < TimeSpan.FromSeconds(1))
        {
            await _semaphore.WaitAsync();
            try
            {
                if (CurrentAudioFileReader != null)
                {
                    CurrentAudioFileReader = null;
                    _isPlaying = false;
                }
            }
            finally
            {
                _semaphore.Release();
            }

            if (e.Exception == null)
            {
                await NextAsync();
            }
            else
            {
                Console.WriteLine("play error: " + e.Exception.Message);
            }
        }
    }

    public AudioFile? GetCurrentAudioFile() => _audioQueue.CurrentAudioFile;

    public IObservable<IReadOnlyCollection<AudioFile>> GetAllAudioFiles() => _audioQueue.AudioFilesObservable;

    private void LoadFile(AudioFile? file)
    {
        lock (_deviceLock)
        {
            _device.Stop();
            CurrentAudioFileReader = null;
            _isPlaying = false;

            if (file == null) return;

            CurrentAudioFileReader = new AudioFileReader(file.FilePath);
            _device.Init(CurrentAudioFileReader);
        }
    }

    public async Task NextAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            _audioQueue.CurrentIndex += 1;
            LoadFile(_audioQueue.CurrentAudioFile);
            await PlayAsync();
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task PrevAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            _audioQueue.CurrentIndex -= 1;
            LoadFile(_audioQueue.CurrentAudioFile);
            await PlayAsync();
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public Task<TimeSpan> GetTotalTimeAsync()
    {
        lock (_deviceLock)
        {
            return Task.FromResult(CurrentAudioFileReader?.TotalTime ?? TimeSpan.Zero);
        }
    }

    public Task<TimeSpan> GetCurrentTimeAsync()
    {
        lock (_deviceLock)
        {
            return Task.FromResult(CurrentAudioFileReader?.CurrentTime ?? TimeSpan.Zero);
        }
    }

    public async Task PlayAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            _isPlaying = true;
            lock (_deviceLock)
            {
                _device.Stop(); // 清除缓存
                if (CurrentAudioFileReader != null)
                    _device.Init(CurrentAudioFileReader);
            }

            _device.Play();
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public Task PauseAsync()
    {
        lock (_deviceLock)
        {
            _isPlaying = false;
            _device.Pause();
        }
        return Task.CompletedTask;
    }

    public async Task SeekAsync(TimeSpan targetTime)
    {
        await _semaphore.WaitAsync();
        try
        {
            lock (_deviceLock)
            {
                if (CurrentAudioFileReader == null) return;

                if (targetTime < TimeSpan.Zero) targetTime = TimeSpan.Zero;
                if (targetTime > CurrentAudioFileReader.TotalTime) targetTime = CurrentAudioFileReader.TotalTime;

                CurrentAudioFileReader.CurrentTime = targetTime;
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public Task<bool> IsPlayingAsync()
    {
        lock (_deviceLock)
        {
            return Task.FromResult(_isPlaying);
        }
    }

    public Task<float> GetVolumeAsync()
    {
        lock (_deviceLock)
        {
            return Task.FromResult(_settingManager.CurrentSetting.Volume);
        }
    }

    public Task SetVolumeAsync(float volume)
    {
        lock (_deviceLock)
        {
            _settingManager.CurrentSetting.Volume = volume;
            _device.Volume = volume;
        }
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        lock (_deviceLock)
        {
            _device.Stop();
            _device.Dispose();
            CurrentAudioFileReader = null;
        }
        _semaphore.Dispose();
    }
}