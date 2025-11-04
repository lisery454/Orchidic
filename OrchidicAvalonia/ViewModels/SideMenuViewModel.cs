using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Windows.Input;
using Orchidic.Models;
using Orchidic.Utils;
using ReactiveUI;

namespace Orchidic.ViewModels;

public class SideMenuViewModel : ViewModelBase
{
    private PageType _pageType = PageType.Playing;
    private ObservableCollection<PageType> _sideMenuItems = [];

    public PageType PageType
    {
        get => _pageType;
        set => this.RaiseAndSetIfChanged(ref _pageType, value);
    }

    public ObservableCollection<PageType> SideMenuItems
    {
        get => _sideMenuItems;
        set => this.RaiseAndSetIfChanged(ref _sideMenuItems, value);
    }

    public ICommand SelectMenuCommand { get; }

    private readonly ObservableAsPropertyHelper<int> _selectIndex;
    public int SelectIndex => _selectIndex.Value;


    public SideMenuViewModel()
    {
        SideMenuItems =
        [
            PageType.Playing,
            PageType.Queue,
            PageType.List,
            PageType.Search,
            PageType.Statistics,
            PageType.Tools,
            PageType.Settings,
        ];

        SelectMenuCommand = ReactiveCommand.Create((PageType type) => { PageType = type; });

        _selectIndex = this
            .WhenAnyValue(vm => vm.PageType)
            .Select(pt => SideMenuItems.IndexOf(pt))
            .ToProperty(this, vm => vm.SelectIndex);
    }
}