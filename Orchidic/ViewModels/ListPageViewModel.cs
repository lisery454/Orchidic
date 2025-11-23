using Orchidic.Models;
using Orchidic.Services.Interfaces;
using Orchidic.Utils;

namespace Orchidic.ViewModels;

public class ListPageViewModel : ViewModelBase
{
    private IPlayerService PlayerService { get; }
    private IAudioQueueService AudioQueueService { get; }
    public IAudioListService AudioListService { get; }

    public ICollectionView AudioListsView { get; }

    public ICommand LoadListCommand { get; }
    public ICommand RemoveListCommand { get; }

    private string _filterText;

    public string FilterText
    {
        get => _filterText;
        set
        {
            this.RaiseAndSetIfChanged(ref _filterText, value);
            AudioListsView.Refresh();
        }
    }


    public ListPageViewModel(
        IPlayerService playerService,
        IAudioQueueService audioQueueService,
        IAudioListService audioListService)
    {
        PlayerService = playerService;
        AudioQueueService = audioQueueService;
        AudioListService = audioListService;

        _filterText = "";

        AudioListsView = CollectionViewSource.GetDefaultView(AudioListService.AudioLists);
        AudioListsView.Filter = FilterAudioList;

        LoadListCommand = ReactiveCommand.Create<AudioList>(list =>
        {
            PlayerService.Stop();
            AudioQueueService.AudioQueue.Load(list);
            if (AudioQueueService.AudioQueue.CurrentAudioFile != null)
                PlayerService.PlayAsync(AudioQueueService.AudioQueue.CurrentAudioFile);
        });

        RemoveListCommand = ReactiveCommand.Create<AudioList>(list => { AudioListService.RemoveAudioList(list); });
    }

    private bool FilterAudioList(object obj)
    {
        if (obj is AudioList list)
        {
            if (string.IsNullOrEmpty(FilterText))
                return true;

            // 忽略大小写匹配
            return list.Name.IndexOf(FilterText, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        return false;
    }
}

public class AudioListCountTextConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return $"总共 {value} 个歌曲列表";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return null;
    }
}