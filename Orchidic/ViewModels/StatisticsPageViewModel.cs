using Orchidic.Models;
using Orchidic.Services.Interfaces;
using Orchidic.Utils;

namespace Orchidic.ViewModels;

public class StatisticsPageViewModel : ViewModelBase
{
    public IStatisticsService StatisticsService { get; private set; }

    public ObservableCollection<AudioFileWithCount> AudioItems { get; } = new();

    public StatisticsPageViewModel(IStatisticsService statisticsService)
    {
        StatisticsService = statisticsService;

        var stats = StatisticsService.CurrentStatistics;

        foreach (var (path, count) in stats.CountMap)
        {
            AudioItems.Add(new AudioFileWithCount(new AudioFile(path), count));
        }

        // 如果 CountMap 会更新，你需要监听并同步：
        stats.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(Statistics.CountMap))
            {
                Application.Current.Dispatcher.Invoke(SyncAudioItems);
            }
        };
    }

    private void SyncAudioItems()
    {
        var stats = StatisticsService.CurrentStatistics;

        // 1. 新增：CountMap 有但 AudioItems 没有的
        foreach (var (path, count) in stats.CountMap)
        {
            if (AudioItems.All(i => i.File.Path != path))
            {
                AudioItems.Add(new AudioFileWithCount(new AudioFile(path), count));
            }
        }

        // 2. 删除：AudioItems 有但 CountMap 没有的
        var toRemove = AudioItems
            .Where(i => !stats.CountMap.ContainsKey(i.File.Path))
            .ToList();

        foreach (var item in toRemove)
            AudioItems.Remove(item);

        // 3. 更新值
        foreach (var item in AudioItems)
        {
            item.Count = stats.CountMap.GetValueOrDefault(item.File.Path, 0);
        }
    }
}

public class StatisticsTotalTimeToStringConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is TimeSpan timeSpan)
        {
            return "当前听歌时长：" + timeSpan.ToString(@"hh\:mm\:ss");
        }

        return Binding.DoNothing;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return Binding.DoNothing;
    }
}