using Orchidic.Utils.LogManager;
using Orchidic.Utils.SettingManager;

namespace Orchidic.Utils.ThemeManager;

public class ThemeManager : IThemeManager
{
    private readonly ILogManager _logManager;

    public ThemeManager(ISettingManager settingManager, ILogManager logManager)
    {
        _logManager = logManager;
        _logManager.Info("Create ThemeManager Success");
        ChangeTheme(settingManager.CurrentSetting.ThemeType);
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

        _logManager.Info($"Change Theme {themeType} Success");
    }
}