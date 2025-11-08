using Orchidic.Models;

namespace Orchidic.Services.Interfaces;

public interface IPlayerService
{
    AudioFile? GetCurrentAudioFile();
    IObservable<IReadOnlyCollection<AudioFile>> GetAllAudioFiles();
    void Next();
    void Prev();
    TimeSpan GetTotalTime();
    TimeSpan GetCurrentTime();
    void Play();
    void Pause();
    void Seek(TimeSpan targetTime);
    bool IsPlaying();
    float GetVolume();
    void SetVolume(float volume);
}