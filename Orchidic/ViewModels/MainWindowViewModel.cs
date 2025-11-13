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

    private readonly IAudioQueueService _audioQueueService;

    public BitmapSource? CurrentBlurCover => _audioQueueService.CurrentBlurCover;

    public MainWindowViewModel(IAudioQueueService audioQueueService)
    {
        _audioQueueService = audioQueueService;
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

        this.WhenAnyValue(x => x._audioQueueService.CurrentBlurCover)
            .Subscribe(_ => { this.RaisePropertyChanged(nameof(CurrentBlurCover));  });
    }
}