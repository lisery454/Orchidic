using Orchidic.Models;

namespace Orchidic.Services.Interfaces;

public interface ISettingService
{
    void Save();
    Setting CurrentSetting { get; set; }
}