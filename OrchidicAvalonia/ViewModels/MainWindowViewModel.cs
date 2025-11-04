using System.Reactive.Linq;
using Microsoft.Extensions.DependencyInjection;
using Orchidic.Models;
using Orchidic.Utils;
using ReactiveUI;

namespace Orchidic.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public SideMenuViewModel SideMenuViewModel { get; } = new();


    private readonly ObservableAsPropertyHelper<ViewModelBase> _currentPageViewModel;
    public ViewModelBase CurrentPageViewModel => _currentPageViewModel.Value;


    public MainWindowViewModel()
    {
        _currentPageViewModel = this.WhenAnyValue(x => x.SideMenuViewModel.PageType)
            .Select<PageType, ViewModelBase>(x =>
            {
                return x switch
                {
                    PageType.Playing => App.Services.GetRequiredService<PlayingPageViewModel>(),
                    PageType.Queue => App.Services.GetRequiredService<QueuePageViewModel>(),
                    PageType.List => App.Services.GetRequiredService<ListPageViewModel>(),
                    PageType.Search => App.Services.GetRequiredService<SearchPageViewModel>(),
                    PageType.Statistics => App.Services.GetRequiredService<StatisticsPageViewModel>(),
                    PageType.Tools => App.Services.GetRequiredService<ToolsPageViewModel>(),
                    PageType.Settings => App.Services.GetRequiredService<SettingsPageViewModel>(),
                    _ => App.Services.GetRequiredService<PlayingPageViewModel>()
                };
            }).ToProperty(this, x => x.CurrentPageViewModel);
    }
}