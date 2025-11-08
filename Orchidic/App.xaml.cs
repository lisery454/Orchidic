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

        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton<PlayingPageViewModel>();
        services.AddSingleton<QueuePageViewModel>();
        services.AddSingleton<ListPageViewModel>();
        services.AddSingleton<SearchPageViewModel>();
        services.AddSingleton<StatisticsPageViewModel>();
        services.AddSingleton<ToolsPageViewModel>();
        services.AddSingleton<SettingsPageViewModel>();

        services.AddSingleton<MainWindow>(sp =>
        {
            var vm = sp.GetRequiredService<MainWindowViewModel>();
            var view = new MainWindow
            {
                DataContext = vm
            };
            return view;
        });
        services.AddSingleton<PlayingPage>(sp =>
        {
            var vm = sp.GetRequiredService<PlayingPageViewModel>();
            var view = new PlayingPage
            {
                DataContext = vm
            };
            return view;
        });
        services.AddSingleton<QueuePage>(sp =>
        {
            var vm = sp.GetRequiredService<QueuePageViewModel>();
            var view = new QueuePage
            {
                DataContext = vm
            };
            return view;
        });
        services.AddSingleton<ListPage>(sp =>
        {
            var vm = sp.GetRequiredService<ListPageViewModel>();
            var view = new ListPage
            {
                DataContext = vm
            };
            return view;
        });
        services.AddSingleton<SearchPage>(sp =>
        {
            var vm = sp.GetRequiredService<SearchPageViewModel>();
            var view = new SearchPage
            {
                DataContext = vm
            };
            return view;
        });
        services.AddSingleton<StatisticsPage>(sp =>
        {
            var vm = sp.GetRequiredService<StatisticsPageViewModel>();
            var view = new StatisticsPage
            {
                DataContext = vm
            };
            return view;
        });
        services.AddSingleton<ToolsPage>(sp =>
        {
            var vm = sp.GetRequiredService<ToolsPageViewModel>();
            var view = new ToolsPage
            {
                DataContext = vm
            };
            return view;
        });
        services.AddSingleton<SettingsPage>(sp =>
        {
            var vm = sp.GetRequiredService<SettingsPageViewModel>();
            var view = new SettingsPage
            {
                DataContext = vm
            };
            return view;
        });

        return services.BuildServiceProvider();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        var settingManager = Services.GetService<ISettingManager>();
        settingManager?.Save();
        Console.WriteLine("Exit");
        base.OnExit(e);
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        var logManager = Services.GetService<ILogManager>();
        logManager?.Info("Application Start Up.");

        var themeManager = Services.GetService<IThemeManager>();
        var settingManager = Services.GetService<ISettingManager>();
        themeManager!.ChangeTheme(settingManager!.CurrentSetting.ThemeType);
        

        var mainWindow = Services.GetService<MainWindow>();

        mainWindow!.Show();
    }
}