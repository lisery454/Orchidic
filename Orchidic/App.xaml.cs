using Orchidic.Services;
using Orchidic.Services.Interfaces;
using Orchidic.Utils;
using Orchidic.Utils.LogManager;
using Orchidic.Utils.SettingManager;
using Orchidic.Utils.ThemeManager;
using Orchidic.ViewModels;
using Orchidic.Views;

namespace Orchidic;

public partial class App
{
    public new static App Current => (App)Application.Current;

    public App()
    {
        ProcessManager.GetProcessLock();
    }

    public IServiceProvider Services { get; } = ConfigureServices();

    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        services.AddSingleton<ISerializer, Serializer>();
        services.AddSingleton<IDeserializer, Deserializer>();

        services.AddSingleton<ILogManager, LogManager>();
        services.AddSingleton<ISettingManager, SettingManager>();
        services.AddSingleton<IThemeManager, ThemeManager>();

        services.AddSingleton<IFileInfoService, FileInfoService>();
        services.AddSingleton<IPlayerService, PlayerService>();

        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton<PlayingPageViewModel>();
        services.AddSingleton<QueuePageViewModel>();
        services.AddSingleton<ListPageViewModel>();
        services.AddSingleton<SearchPageViewModel>();
        services.AddSingleton<StatisticsPageViewModel>();
        services.AddSingleton<ToolsPageViewModel>();
        services.AddSingleton<SettingsPageViewModel>();

        services.AddSingleton<MainWindow>();
        services.AddSingleton<PlayingPage>();
        services.AddSingleton<QueuePage>();
        services.AddSingleton<ListPage>();
        services.AddSingleton<SearchPage>();
        services.AddSingleton<StatisticsPage>();
        services.AddSingleton<ToolsPage>();
        services.AddSingleton<SettingsPage>();

        return services.BuildServiceProvider();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        // save setting
        var settingManager = Services.GetService<ISettingManager>();
        settingManager?.Save();
        
        // log
        var logManager = Services.GetService<ILogManager>();
        logManager?.Info("Application Exit.");
        base.OnExit(e);
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        // log
        var logManager = Services.GetService<ILogManager>();
        logManager?.Info("Application Start Up.");

        // set theme
        var themeManager = Services.GetService<IThemeManager>();
        var settingManager = Services.GetService<ISettingManager>();
        themeManager!.ChangeTheme(settingManager!.CurrentSetting.ThemeType);

        // start window
        var mainWindow = Services.GetService<MainWindow>();
        mainWindow!.Show();
    }
}