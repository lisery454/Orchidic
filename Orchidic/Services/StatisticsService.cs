using Orchidic.Models;
using Orchidic.Services.Interfaces;
using Orchidic.Utils;

namespace Orchidic.Services;

public class StatisticsService : IStatisticsService
{
    private Statistics _currentStatistics;

    public Statistics CurrentStatistics
    {
        get => _currentStatistics;
        set
        {
            _currentStatistics = value;
            Save();
        }
    }

    private readonly ISerializer _serializer;
    private readonly IDeserializer _deserializer;
    private readonly ILogService _logService;

    public StatisticsService(ILogService logService)
    {
        _serializer = new Serializer();
        _deserializer = new Deserializer();
        _logService = logService;

        CheckIfStatisticsFileExists();
        try
        {
            _currentStatistics =
                _deserializer.Deserialize<Statistics>(File.ReadAllText(ProgramConstants.StatisticsPath));
            _logService.Info("Load Statistics Success.");
        }
        catch (Exception e)
        {
            _logService.Warning($"Load Statistics Fail. Exception: {e}");
            _currentStatistics = new Statistics();
            _logService.Info("Create Default Statistics.");
            Save();
        }
    }

    public void Save()
    {
        CheckIfStatisticsFileExists();
        var serialize = _serializer.Serialize(CurrentStatistics);
        using var sw = new StreamWriter(ProgramConstants.StatisticsPath);
        sw.WriteLine(serialize);
        _logService.Info("Save Statistics Success.");
    }

    private void CheckIfStatisticsFileExists()
    {
        if (File.Exists(ProgramConstants.StatisticsPath)) return;

        using var fileStream = File.Create(ProgramConstants.StatisticsPath);
        using var sw = new StreamWriter(fileStream);
        var serialize = _serializer.Serialize(new Statistics());
        sw.WriteLine(serialize);
    }
}