using Orchidic.Utils;
using Orchidic.Utils.ThemeManager;
using Orchidic.ViewModels;

namespace Orchidic;

public partial class App : Application
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
        services.AddSingleton<IThemeManager, ThemeManager>();


        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton<MainWindow>(sp =>
        {
            var mainWindowViewModel = sp.GetService<MainWindowViewModel>()!;
            var mainWindow = new MainWindow
            {
                DataContext = mainWindowViewModel
            };
            return mainWindow;
        });

        return services.BuildServiceProvider();
    }

    private void Application_Startup(object sender, StartupEventArgs e)
    {
        // var logger = Services.GetService<ILogger>();
        // logger?.Information("Application Start Up.");
        // _ = Services.GetService<ILanguageManager>();
        _ = Services.GetService<IThemeManager>();

        // var loginWindow = Services.GetService<LoginWindow>();
        // loginWindow!.Show();
        var mainWindow = Services.GetService<MainWindow>();
        mainWindow!.Show();
    }
}