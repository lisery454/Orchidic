using System;
using NAudio.Wave;

namespace Orchidic.Service;

public class PlayerService : IPlayerService, IDisposable
{
    private readonly WaveOutEvent _device = new();
    private AudioFileReader? _audioFile;

    public void LoadFile(string path)
    {
        _audioFile = new AudioFileReader(path);
        _device.Init(_audioFile);
        _device.Volume = 0.3f;
    }

    public TimeSpan GetTotalTime()
    {
        return _audioFile?.TotalTime ?? TimeSpan.Zero;
    }

    public TimeSpan GetCurrentTime()
    {
        return _audioFile?.CurrentTime ?? TimeSpan.Zero;
    }

    public void Play()
    {
        _device.Play();
    }

    public void Pause()
    {
        _device.Pause();
    }

    public void Seek(TimeSpan targetTime)
    {
        if (_audioFile == null) return;

        if (targetTime < TimeSpan.Zero) targetTime = TimeSpan.Zero;
        if (targetTime > _audioFile.TotalTime) targetTime = _audioFile.TotalTime;

        _audioFile.CurrentTime = targetTime;
    }

    public bool IsPlaying()
    {
        return _device.PlaybackState == PlaybackState.Playing;
    }

    public void Dispose()
    {
        _device.Dispose();
        _audioFile?.Dispose();
    }
}