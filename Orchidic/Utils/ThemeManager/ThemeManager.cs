using Orchidic.Utils.LogManager;
using Orchidic.Utils.SettingManager;

namespace Orchidic.Utils.ThemeManager;

public class ThemeManager : IThemeManager
{
    private readonly ILogManager _logManager;
    private readonly ISettingManager _settingManager;

    public ThemeManager(ISettingManager settingManager, ILogManager logManager)
    {
        _logManager = logManager;
        _settingManager = settingManager;
        _logManager.Info("Create ThemeManager Success");
    }

    public void ChangeTheme(ThemeType themeType)
    {
        var themeFilePath = themeType switch
        {
            ThemeType.LIGHT => "Styles/ThemeLight.xaml",
            ThemeType.DARK => "Styles/ThemeDark.xaml",
            _ => throw new ArgumentOutOfRangeException(nameof(themeType), themeType, null)
        };

        var resourceDictionary = new ResourceDictionary
        {
            Source = new Uri(themeFilePath, UriKind.Relative)
        };

        Application.Current.Resources.MergedDictionaries[0] = resourceDictionary;
        
        ThemeChanged?.Invoke(null, themeType);
        _logManager.Info($"Change Theme {themeType} Success");
        _settingManager.CurrentSetting.ThemeType = themeType;
    }

    public ThemeType GetCurrentTheme()
    {
        return _settingManager.CurrentSetting.ThemeType;
    }

    public event EventHandler<ThemeType>? ThemeChanged;
}