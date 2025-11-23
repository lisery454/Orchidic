using Orchidic.Models;

namespace Orchidic.Services.Interfaces;

public interface IAudioListService
{
    public ObservableCollection<AudioList> AudioLists { get; }

    void AddAudioList(string name, string[] paths);

    void RemoveAudioList(AudioList audioList);
}