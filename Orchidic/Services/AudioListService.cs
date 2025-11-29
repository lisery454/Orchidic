using Orchidic.Models;
using Orchidic.Services.Interfaces;

namespace Orchidic.Services;

public class AudioListService : IAudioListService
{
    public ObservableCollection<AudioList> AudioLists { get; }

    private readonly ISettingService _settingService;


    public AudioListService(ISettingService settingService)
    {
        _settingService = settingService;

        AudioLists = [];
        foreach (var setting in settingService.CurrentSetting.AudioListInfo)
        {
            AudioLists.Add(new AudioList(setting.Name, setting.Path));
        }
    }

    public void AddAudioList(string name, string path)
    {
        AudioLists.Add(new AudioList(name, path));
        _settingService.CurrentSetting.AudioListInfo =
            AudioLists.Select(list => new AudioListSetting(list.Name, list.DirectoryPath)).ToArray();
    }

    public void RemoveAudioList(AudioList audioList)
    {
        AudioLists.Remove(audioList);
        _settingService.CurrentSetting.AudioListInfo =
            AudioLists.Select(list => new AudioListSetting(list.Name, list.DirectoryPath)).ToArray();
    }
}