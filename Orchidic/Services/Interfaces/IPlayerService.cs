using Orchidic.Models;

namespace Orchidic.Services.Interfaces;

public interface IPlayerService
{
    AudioFile? GetCurrentAudioFile();
    IObservable<IReadOnlyCollection<AudioFile>> GetAllAudioFiles();

    Task NextAsync();
    Task PrevAsync();
    Task PlayAsync();
    Task PauseAsync();
    Task SeekAsync(TimeSpan targetTime);

    Task<TimeSpan> GetTotalTimeAsync();
    Task<TimeSpan> GetCurrentTimeAsync();

    Task<bool> IsPlayingAsync();
    Task<float> GetVolumeAsync();
    Task SetVolumeAsync(float volume);
}