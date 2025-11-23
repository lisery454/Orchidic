using Orchidic.Models;
using Orchidic.Services.Interfaces;
using Orchidic.Utils;
using Orchidic.ViewModels.Components;

namespace Orchidic.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public SideMenuViewModel SideMenuViewModel { get; } = new();

    private readonly ObservableAsPropertyHelper<ViewModelBase> _currentPageViewModel;
    public ViewModelBase CurrentPageViewModel => _currentPageViewModel.Value;

    public IAudioQueueService AudioQueueService { get; }
    public IGlobalService GlobalService { get; }

    public ICommand MinimizeCommand { get; }
    public ICommand ToggleMaximizeCommand { get; }
    public ICommand CloseCommand { get; }


    public MainWindowViewModel(IAudioQueueService audioQueueService, IGlobalService globalService)
    {
        AudioQueueService = audioQueueService;
        GlobalService = globalService;
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

        MinimizeCommand = ReactiveCommand.Create<Window>(window => { window.WindowState = WindowState.Minimized; });
        ToggleMaximizeCommand = ReactiveCommand.Create<Window>(window =>
        {
            window.WindowState = window.WindowState == WindowState.Maximized
                ? WindowState.Normal
                : WindowState.Maximized;
        });
        CloseCommand = ReactiveCommand.Create<Window>(window => { window.Close(); });
    }
}