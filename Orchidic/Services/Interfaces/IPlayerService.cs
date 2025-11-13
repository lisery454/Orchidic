using Orchidic.Models;

namespace Orchidic.Services.Interfaces;

public interface IPlayerService : IDisposable
{
    bool IsPlaying { get; }
    AudioFile? CurrentAudioFile { get; }
    TimeSpan CurrentTime { get; }
    float Progress { get; set; }
    TimeSpan TotalTime { get; }
    float Volume { get; set; }

    IObservable<TimeSpan> CurrentTimeObservable { get; }
    IObservable<TimeSpan> TotalTimeObservable { get; }
    IObservable<float> ProgressObservable { get; }
    IObservable<float> VolumeObservable { get; }

    Task PlayAsync(AudioFile audioFile);
    void Pause();
    void Resume();
    void Stop();
    void Seek(float progress);

    event EventHandler? PlaybackEnded;
}