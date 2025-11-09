using Orchidic.Models;
using Orchidic.Utils;
using Orchidic.Utils.ThemeManager;
using Orchidic.ViewModels.Components;

namespace Orchidic.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public SideMenuViewModel SideMenuViewModel { get; } = new();

    private readonly ObservableAsPropertyHelper<ViewModelBase> _currentPageViewModel;
    public ViewModelBase CurrentPageViewModel => _currentPageViewModel.Value;

    private readonly ObservableAsPropertyHelper<BitmapSource?> _cover;
    public BitmapSource? Cover => _cover.Value;

    private static IThemeManager themeManager => App.Current.Services.GetService<IThemeManager>()!;

    public MainWindowViewModel()
    {
        _currentPageViewModel = this.WhenAnyValue(x => x.SideMenuViewModel.PageType)
            .Select<PageType, ViewModelBase>(x =>
            {
                return x switch
                {
                    PageType.Playing => App.Current.Services.GetRequiredService<PlayingPageViewModel>(),
                    PageType.Queue => App.Current.Services.GetRequiredService<QueuePageViewModel>(),
                    PageType.List => App.Current.Services.GetRequiredService<ListPageViewModel>(),
                    PageType.Search => App.Current.Services.GetRequiredService<SearchPageViewModel>(),
                    PageType.Statistics => App.Current.Services.GetRequiredService<StatisticsPageViewModel>(),
                    PageType.Tools => App.Current.Services.GetRequiredService<ToolsPageViewModel>(),
                    PageType.Settings => App.Current.Services.GetRequiredService<SettingsPageViewModel>(),
                    _ => App.Current.Services.GetRequiredService<PlayingPageViewModel>()
                };
            }).ToProperty(this, x => x.CurrentPageViewModel);

        App.Current.Services.GetService<PlayingPageViewModel>().WhenAnyValue(x => x.Cover)
            // 在后台线程生成模糊图像
            .SelectMany(original =>
                Observable.FromAsync(() =>
                        RunInSta(() => SmoothImageScaler.ScaleWithSmoothBlur(original!, 800, 100))
                    )
                    .Catch<BitmapSource, Exception>(ex =>
                    {
                        Console.WriteLine($"Blur generation error: {ex.Message}");
                        return Observable.Empty<BitmapSource>();
                    })
            )
            // 切回 UI 线程更新
            .ObserveOn(RxApp.MainThreadScheduler)
            .ToProperty(this, x => x.Cover, out _cover);
    }

    private static Task<T> RunInSta<T>(Func<T> func)
    {
        var tcs = new TaskCompletionSource<T>();

        var thread = new Thread(() =>
        {
            try
            {
                var result = func();
                tcs.SetResult(result);
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }
        });

        thread.SetApartmentState(ApartmentState.STA);
        thread.IsBackground = true;
        thread.Start();

        return tcs.Task;
    }
}