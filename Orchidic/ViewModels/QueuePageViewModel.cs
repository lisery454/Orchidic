using Orchidic.Models;
using Orchidic.Services.Interfaces;
using Orchidic.Utils;


namespace Orchidic.ViewModels;

public class QueuePageViewModel : ViewModelBase
{
    private IPlayerService PlayerService { get; }
    public IAudioQueueService AudioQueueService { get; }

    public ICommand PlayCommand { get; }

    public ICommand LocateCommand { get; }

    public ICollectionView AudioFilesView { get; }

    private string _filterText;

    public string FilterText
    {
        get => _filterText;
        set
        {
            this.RaiseAndSetIfChanged(ref _filterText, value);
            AudioFilesView.Refresh();
        }
    }

    public ICommand RemoveFileCommand { get; }
    public ICommand AddFilesCommand { get; }

    public QueuePageViewModel(IPlayerService playerService, IAudioQueueService audioQueueService)
    {
        PlayerService = playerService;
        AudioQueueService = audioQueueService;

        PlayCommand = ReactiveCommand.CreateFromTask<AudioFile>(async file =>
        {
            if (PlayerService.CurrentAudioFile == file)
            {
                if (!PlayerService.IsPlaying)
                {
                    PlayerService.Resume();
                }
            }
            else
            {
                AudioQueueService.AudioQueue.TrySetCurrentAudioFile(file);
                await PlayerService.PlayAsync(file);
            }
        });

        RemoveFileCommand = ReactiveCommand.Create<AudioFile>((file) =>
        {
            if (PlayerService.CurrentAudioFile == file)
            {
                AudioQueueService.AudioQueue.Remove(file);
                if (AudioQueueService.AudioQueue.CurrentAudioFile != null)
                    playerService.PlayAsync(AudioQueueService.AudioQueue.CurrentAudioFile);
                else playerService.Stop();
            }
            else
            {
                AudioQueueService.AudioQueue.Remove(file);
            }
        });

        AddFilesCommand = ReactiveCommand.Create<IEnumerable<AudioFile>>(files =>
        {
            AudioQueueService.AudioQueue.Add(files);
            if (AudioQueueService.AudioQueue.CurrentAudioFile != null)
                playerService.PlayAsync(AudioQueueService.AudioQueue.CurrentAudioFile);
        });

        var canLocate = this
            .WhenAnyValue(x => x.AudioQueueService.AudioQueue.CurrentAudioFile)
            .Select(file => file != null);
        LocateCommand = ReactiveCommand.Create<ListBox>(listbox =>
        {
            if (AudioQueueService.AudioQueue.CurrentAudioFile == null) return;
            FilterText = "";
            listbox.ScrollIntoView(AudioQueueService.AudioQueue.CurrentAudioFile);
        }, canLocate);

        _filterText = "";

        AudioFilesView = CollectionViewSource.GetDefaultView(AudioQueueService.AudioQueue.AudioFiles);
        AudioFilesView.Filter = FilterAudioFiles;
    }

    private bool FilterAudioFiles(object obj)
    {
        if (obj is AudioFile file)
        {
            if (string.IsNullOrEmpty(FilterText))
                return true;

            // 忽略大小写匹配
            return file.Name.IndexOf(FilterText, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        return false;
    }
}

public class AudioFileEqualConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        var audioFile0 = values[0] as AudioFile;
        var audioFile1 = values[1] as AudioFile;
        return audioFile0 == audioFile1;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        return [];
    }
}

public class AudioCountTextConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return $"总共 {value} 首";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return null;
    }
}