using Orchidic.Models;

namespace Orchidic.Services.Interfaces;

public interface IStatisticsService
{
    void Save();
    Statistics CurrentStatistics { get; set; }
}