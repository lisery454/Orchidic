using Orchidic.Utils.ThemeManager;

namespace Orchidic.Utils.SettingManager;

public class Setting
{
    public ThemeType ThemeType { get; set; } = ThemeType.LIGHT;

    public float Volume { get; set; } = 0.3f;
}