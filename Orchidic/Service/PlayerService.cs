using System;
using NAudio.Wave;
using Orchidic.Models;

namespace Orchidic.Service;

public class PlayerService : IPlayerService, IDisposable
{
    private readonly WaveOutEvent _device = new();
    private AudioFileReader? _currentAudioFileReader;
    private readonly AudioQueue _audioQueue;

    public PlayerService()
    {
        _audioQueue = new AudioQueue();

        LoadFile(_audioQueue.CurrentAudioFile);
    }

    public AudioFile? GetCurrentAudioFile()
    {
        return _audioQueue.CurrentAudioFile;
    }

    public void LoadFile(AudioFile? file)
    {
        _device.Stop();
        _currentAudioFileReader?.Dispose();
        _currentAudioFileReader = null;

        if (file == null) return;
        
        _currentAudioFileReader = new AudioFileReader(file.path);
        _device.Init(_currentAudioFileReader);
        _device.Volume = 0.3f;
    }

    public void Next()
    {
        _audioQueue.CurrentIndex += 1;
        _device.Stop();
        LoadFile(_audioQueue.CurrentAudioFile);
        Play();
    }

    public void Prev()
    {
        _audioQueue.CurrentIndex -= 1;
        LoadFile(_audioQueue.CurrentAudioFile);
        Play();
    }

    public TimeSpan GetTotalTime()
    {
        return _currentAudioFileReader?.TotalTime ?? TimeSpan.Zero;
    }

    public TimeSpan GetCurrentTime()
    {
        return _currentAudioFileReader?.CurrentTime ?? TimeSpan.Zero;
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
        if (_currentAudioFileReader == null) return;

        if (targetTime < TimeSpan.Zero) targetTime = TimeSpan.Zero;
        if (targetTime > _currentAudioFileReader.TotalTime) targetTime = _currentAudioFileReader.TotalTime;

        _currentAudioFileReader.CurrentTime = targetTime;
    }

    public bool IsPlaying()
    {
        return _device.PlaybackState == PlaybackState.Playing;
    }

    public void Dispose()
    {
        _device.Stop();
        _device.Dispose();
        _currentAudioFileReader?.Dispose();
    }
}