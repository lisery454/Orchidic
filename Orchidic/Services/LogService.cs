using Orchidic.Services.Interfaces;
using Orchidic.Utils;

namespace Orchidic.Services;

public class LogService : ILogService
{
    private readonly Logger _logger = new LoggerConfiguration().MinimumLevel.Debug().WriteTo
        .File(ProgramConstants.LogPath)
        .CreateLogger();

    public LogService()
    {
        Info("Create LogManager Success");
    }

    public void Info(string message)
    {
        _logger.Information("{Message}", message);
    }

    public void Warning(string message)
    {
        _logger.Warning("{Message}", message);
    }

    public void Error(string message)
    {
        _logger.Error("{Message}", message);
    }
}