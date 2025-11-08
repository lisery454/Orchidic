namespace Orchidic.Utils.ThemeManager;

public interface IThemeManager
{
    void ChangeTheme(ThemeType themeType);
    ThemeType GetCurrentTheme();
    event EventHandler<ThemeType>? ThemeChanged;
}