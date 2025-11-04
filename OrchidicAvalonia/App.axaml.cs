using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using Microsoft.Extensions.DependencyInjection;
using Orchidic.Services;
using Orchidic.Services.Interfaces;
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

        services.AddSingleton<IPlayerService, PlayerService>();
        services.AddSingleton<IFileInfoService, FileInfoService>();

        Services = services.BuildServiceProvider();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            WarmUpPages();
            desktop.MainWindow = Services.GetService<MainWindow>();
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void WarmUpPages()
    {
        Task.Run(() =>
        {
            Dispatcher.UIThread.Post(() =>
            {
                try
                {
                    // ⚡ 强制实例化所有页面（Avalonia 会在内部创建视觉树）
                    _ = Services.GetRequiredService<PlayingPage>();
                    _ = Services.GetRequiredService<QueuePage>();
                    _ = Services.GetRequiredService<ListPage>();
                    _ = Services.GetRequiredService<SearchPage>();
                    _ = Services.GetRequiredService<StatisticsPage>();
                    _ = Services.GetRequiredService<ToolsPage>();
                    _ = Services.GetRequiredService<SettingsPage>();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"预热页面失败: {ex.Message}");
                }
            });
        });
    }

    private static void WarmUpSkiaText()
    {
        Dispatcher.UIThread.Post(() =>
        {
            // 创建一个不可见的 TextBlock
            var tb = new TextBlock
            {
                Text = "Warm up Skia font cache你好こにちは",
                FontFamily = new FontFamily("NotoSansMono"),
                FontSize = 22
            };

            // 渲染一次到临时控件（不必添加到UI树）
            tb.Measure(Size.Infinity);
            tb.Arrange(new Rect(0, 0, 100, 20));
        });
    }
}