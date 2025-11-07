namespace Orchidic.Utils.SettingManager;

public interface ISettingManager
{
    void Save();
    Setting CurrentSetting { get; set; }
}