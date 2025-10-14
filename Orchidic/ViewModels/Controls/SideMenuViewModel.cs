using System.Collections.ObjectModel;
using System.Reactive;
using System.Windows.Input;
using Orchidic.Models;
using ReactiveUI;

namespace Orchidic.ViewModels;

public class SideMenuViewModel : ReactiveObject
{
    private PageType _pageType = PageType.Playing;
    private ObservableCollection<SideMenuItemViewModel> _sideMenuItems = [];

    public PageType PageType
    {
        get => _pageType;
        set => this.RaiseAndSetIfChanged(ref _pageType, value);
    }

    public ObservableCollection<SideMenuItemViewModel> SideMenuItems
    {
        get => _sideMenuItems;
        set => this.RaiseAndSetIfChanged(ref _sideMenuItems, value);
    }


    public SideMenuViewModel()
    {
        var selectItemCommand = ReactiveCommand.Create<PageType>((PageType pageType) => { PageType = pageType; });
        SideMenuItems =
        [
            new SideMenuItemViewModel("正在播放", PageType.Playing, selectItemCommand),
            new SideMenuItemViewModel("播放队列", PageType.Queue, selectItemCommand),
            new SideMenuItemViewModel("歌单", PageType.List, selectItemCommand),
            new SideMenuItemViewModel("搜索", PageType.Search, selectItemCommand),
            new SideMenuItemViewModel("统计", PageType.Statistics, selectItemCommand),
            new SideMenuItemViewModel("工具", PageType.Tools, selectItemCommand),
            new SideMenuItemViewModel("设置", PageType.Settings, selectItemCommand)
        ];

        this.WhenAnyValue(x => x.PageType)
            .Subscribe(Observer.Create<PageType>(onNext: UpdateMenuSelection));

        UpdateMenuSelection(PageType);
    }

    private void UpdateMenuSelection(PageType current)
    {
        foreach (var item in SideMenuItems)
            item.IsSelected = item.Type == current;
    }
}