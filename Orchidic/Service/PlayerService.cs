using System;
using NAudio.Wave;
using Orchidic.Models;

namespace Orchidic.Service;

public class PlayerService : IPlayerService, IDisposable
{
    private readonly WaveOutEvent _device;
    private AudioFileReader? _currentAudioFileReader;
    private readonly AudioQueue _audioQueue;
    private float _currentVolume;
    private readonly object _deviceLock = new();
    private bool _isPlaying;

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

    public PlayerService()
    {
        _audioQueue = new AudioQueue();

        _device = new WaveOutEvent();
        _currentVolume = 0.3f;
        _device.Volume = _currentVolume;

        _device.PlaybackStopped += OnPlaybackStopped;
        _isPlaying = false;

        LoadFile(_audioQueue.CurrentAudioFile);
    }


    private void OnPlaybackStopped(object? sender, StoppedEventArgs e)
    {
        // 如果是播放到末尾的停止
        if ((GetTotalTime() - GetCurrentTime()).Duration() < TimeSpan.FromSeconds(1))
        {
            lock (_deviceLock)
            {
                if (CurrentAudioFileReader != null)
                {
                    CurrentAudioFileReader = null;
                    _isPlaying = false;
                }
            }

            if (e.Exception == null)
            {
                Next();
            }
            else
            {
                Console.WriteLine("play error: " + e.Exception.Message);
            }
        }
    }

    public AudioFile? GetCurrentAudioFile()
    {
        return _audioQueue.CurrentAudioFile;
    }

    public void LoadFile(AudioFile? file)
    {
        lock (_deviceLock)
        {
            _device.Stop();
            CurrentAudioFileReader = null;
            _isPlaying = false;

            if (file == null) return;

            CurrentAudioFileReader = new AudioFileReader(file.path);
            _device.Init(CurrentAudioFileReader);
        }
    }

    public void Next()
    {
        _audioQueue.CurrentIndex += 1;
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
        return CurrentAudioFileReader?.TotalTime ?? TimeSpan.Zero;
    }

    public TimeSpan GetCurrentTime()
    {
        return CurrentAudioFileReader?.CurrentTime ?? TimeSpan.Zero;
    }

    public void Play()
    {
        _isPlaying = true;
        lock (_deviceLock)
        {
            _device.Stop(); // 清除缓存
            _device.Init(CurrentAudioFileReader);
        }

        _device.Play();
    }

    public void Pause()
    {
        _isPlaying = false;
        _device.Pause();
    }


    public void Seek(TimeSpan targetTime)
    {
        lock (_deviceLock)
        {
            if (CurrentAudioFileReader == null) return;

            if (targetTime < TimeSpan.Zero) targetTime = TimeSpan.Zero;
            if (targetTime > CurrentAudioFileReader.TotalTime) targetTime = CurrentAudioFileReader.TotalTime;

            CurrentAudioFileReader.CurrentTime = targetTime;
        }
    }

    public bool IsPlaying()
    {
        return _isPlaying;
    }

    public float GetVolume()
    {
        return _currentVolume;
    }

    public void SetVolume(float volume)
    {
        _currentVolume = volume;
        _device.Volume = _currentVolume;
    }

    public void Dispose()
    {
        _device.Stop();
        _device.Dispose();
        CurrentAudioFileReader = null;
    }
}