using Orchidic.Services;
using Orchidic.Services.Interfaces;
using Orchidic.Utils;
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

        // services
        services.AddSingleton<ILogService, LogService>();
        services.AddSingleton<ISettingService, SettingService>();
        services.AddSingleton<IGlobalService, GlobalService>();
        services.AddSingleton<IAudioListService, AudioListService>();
        services.AddSingleton<IAudioQueueService, AudioQueueService>();
        services.AddSingleton<IPlayerService, PlayerService>();

        // view-models
        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton<PlayingPageViewModel>();
        services.AddSingleton<QueuePageViewModel>();
        services.AddSingleton<ListPageViewModel>();
        services.AddSingleton<SearchPageViewModel>();
        services.AddSingleton<StatisticsPageViewModel>();
        services.AddSingleton<ToolsPageViewModel>();
        services.AddSingleton<SettingsPageViewModel>();

        // view
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
        var settingService = Services.GetService<ISettingService>();
        settingService?.Save();

        // log
        var logService = Services.GetService<ILogService>();
        logService?.Info("Application Exit.");
        base.OnExit(e);
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        // log
        var logService = Services.GetService<ILogService>();
        logService?.Info("Application Start Up.");

        // start window
        var mainWindow = Services.GetService<MainWindow>();
        mainWindow!.Show();
    }
}