using Orchidic.Models;
using Orchidic.Services.Interfaces;

namespace Orchidic.Services;

public class AudioListService : IAudioListService
{
    public ObservableCollection<AudioList> AudioLists { get; }

    private ISettingService _settingService;


    public AudioListService(ISettingService settingService)
    {
        _settingService = settingService;

        AudioLists = [];
        foreach (var (name, paths) in settingService.CurrentSetting.AudioListInfo)
        {
            AudioLists.Add(new AudioList(name, paths));
        }
    }

    public void AddAudioList(string name, string[] paths)
    {
        AudioLists.Add(new AudioList(name, paths));
        _settingService.CurrentSetting.AudioListInfo =
            AudioLists.Select(list => (list.Name, list.AudioFilePaths.ToArray())).ToArray();
    }

    public void RemoveAudioList(AudioList audioList)
    {
        AudioLists.Remove(audioList);
        _settingService.CurrentSetting.AudioListInfo =
            AudioLists.Select(list => (list.Name, list.AudioFilePaths.ToArray())).ToArray();
    }
}