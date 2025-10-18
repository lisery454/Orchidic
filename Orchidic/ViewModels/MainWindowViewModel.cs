using System.Reactive.Linq;
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
                    PageType.Playing => new PlayingPageViewModel(),
                    PageType.Queue => new QueuePageViewModel(),
                    PageType.List => new ListPageViewModel(),
                    PageType.Search => new SearchPageViewModel(),
                    PageType.Statistics => new StatisticsPageViewModel(),
                    PageType.Tools => new ToolsPageViewModel(),
                    PageType.Settings => new SettingsPageViewModel(),
                    _ => new PlayingPageViewModel()
                };
            }).ToProperty(this, x => x.CurrentPageViewModel);
    }
}