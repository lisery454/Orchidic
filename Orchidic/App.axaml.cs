using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Orchidic.Service;
using Orchidic.ViewModels;
using Orchidic.Views;

namespace Orchidic;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = default!;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var services = new ServiceCollection();

        services.AddSingleton<MainWindowViewModel>();
        services.AddTransient<PlayingPageViewModel>();
        services.AddTransient<QueuePageViewModel>();
        services.AddTransient<ListPageViewModel>();
        services.AddTransient<SearchPageViewModel>();
        services.AddTransient<StatisticsPageViewModel>();
        services.AddTransient<ToolsPageViewModel>();
        services.AddTransient<SettingsPageViewModel>();

        services.AddSingleton<IPlayerService, PlayerService>();

        Services = services.BuildServiceProvider();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = Services.GetRequiredService<MainWindowViewModel>()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}