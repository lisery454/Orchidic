using Orchidic.Models;
using Orchidic.Services.Interfaces;
using Orchidic.Utils.SettingManager;

namespace Orchidic.Services;

public class AudioListService : IAudioListService
{
    public ObservableCollection<AudioList> AudioLists { get; }

    private ISettingManager _settingManager;


    public AudioListService(ISettingManager settingManager)
    {
        _settingManager = settingManager;

        AudioLists = [];
        foreach (var (name, paths) in settingManager.CurrentSetting.AudioListInfo)
        {
            AudioLists.Add(new AudioList(name, paths));
        }
    }

    public void AddAudioList(string name, string[] paths)
    {
        AudioLists.Add(new AudioList(name, paths));
        _settingManager.CurrentSetting.AudioListInfo =
            AudioLists.Select(list => (list.Name, list.AudioFilePaths.ToArray())).ToArray();
    }

    public void RemoveAudioList(AudioList audioList)
    {
        AudioLists.Remove(audioList);
        _settingManager.CurrentSetting.AudioListInfo =
            AudioLists.Select(list => (list.Name, list.AudioFilePaths.ToArray())).ToArray();
    }
}