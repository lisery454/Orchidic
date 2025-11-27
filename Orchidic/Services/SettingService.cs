using Orchidic.Models;
using Orchidic.Services.Interfaces;
using Orchidic.Utils;

namespace Orchidic.Services;

public class SettingService : ISettingService
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
    private readonly ILogService _logService;

    public SettingService(ILogService logService)
    {
        _serializer = new Serializer();
        _deserializer = new Deserializer();
        _logService = logService;

        CheckIfSettingFileExists();
        try
        {
            _currentSetting = _deserializer.Deserialize<Setting>(File.ReadAllText(ProgramConstants.SettingPath));
            _logService.Info("Load Setting Success.");
        }
        catch (Exception e)
        {
            _logService.Warning($"Load Setting Fail. Exception: {e}");
            _currentSetting = new Setting();
            _logService.Info("Create Default Setting.");
            Save();
        }
    }

    public void Save()
    {
        CheckIfSettingFileExists();
        var serialize = _serializer.Serialize(CurrentSetting);
        using var sw = new StreamWriter(ProgramConstants.SettingPath);
        sw.WriteLine(serialize);
        _logService.Info("Save Setting Success.");
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