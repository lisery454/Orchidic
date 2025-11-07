using Orchidic.Utils.LogManager;

namespace Orchidic.Utils.SettingManager;

public class SettingManager : ISettingManager
{
    private Setting _currentSetting;

    public Setting CurrentSetting
    {
        get => _currentSetting;
        set
        {
            _currentSetting = value;
            Save();
        }
    }

    private readonly ISerializer _serializer;
    private readonly IDeserializer _deserializer;
    private readonly ILogManager _logManager;

    public SettingManager(ISerializer serializer, IDeserializer deserializer, ILogManager logManager)
    {
        _serializer = serializer;
        _deserializer = deserializer;
        _logManager = logManager;

        CheckIfSettingFileExists();
        try
        {
            _currentSetting = _deserializer.Deserialize<Setting>(File.ReadAllText(ProgramConstants.SettingPath));
            _logManager.Info("Load Setting Success.");
        }
        catch (Exception e)
        {
            _logManager.Warning($"Load Setting Fail. Exception: {e}");
            _currentSetting = new Setting();
            _logManager.Info("Create Default Setting.");
            Save();
        }
    }

    public void Save()
    {
        CheckIfSettingFileExists();
        var serialize = _serializer.Serialize(CurrentSetting);
        using var sw = new StreamWriter(ProgramConstants.SettingPath);
        sw.WriteLine(serialize);
        _logManager.Info("Save Setting Success.");
    }

    private void CheckIfSettingFileExists()
    {
        if (File.Exists(ProgramConstants.SettingPath)) return;

        using var fileStream = File.Create(ProgramConstants.SettingPath);
        using var sw = new StreamWriter(fileStream);
        var serialize = _serializer.Serialize(new Setting());
        sw.WriteLine(serialize);
    }
}