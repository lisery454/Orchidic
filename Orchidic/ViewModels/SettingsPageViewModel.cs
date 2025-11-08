using Orchidic.Utils;
using Orchidic.Utils.ThemeManager;


namespace Orchidic.ViewModels;

public class SettingsPageViewModel : ViewModelBase
{
    public ICommand ToggleThemeCommand { get; }

    public SettingsPageViewModel(IThemeManager themeManager)
    {
        ToggleThemeCommand = ReactiveCommand.Create(() =>
        {
            themeManager.ChangeTheme(themeManager.GetCurrentTheme() == ThemeType.DARK
                ? ThemeType.LIGHT
                : ThemeType.DARK);

            RestartApplication();
        });
    }

    private static void RestartApplication()
    {
        // 获取当前可执行文件路径
        var exePath = Process.GetCurrentProcess().MainModule?.FileName;
        if (string.IsNullOrEmpty(exePath)) return;

        // 启动新进程
        Process.Start(exePath);

        // 关闭当前应用
        Application.Current.Shutdown();
    }
}