using Orchidic.Utils;

namespace Orchidic.ViewModels;

public class SettingsPageViewModel : ViewModelBase
{
    private long _folderSize;

    private long FolderSize
    {
        get => _folderSize;
        set
        {
            if (_folderSize != value)
            {
                _folderSize = value;
                this.RaisePropertyChanged();
                this.RaisePropertyChanged(nameof(FolderSizeReadable));
            }
        }
    }

    public ICommand ClearCacheCommand { get; }

    public SettingsPageViewModel()
    {
        _ = new FolderSizeMonitor(ProgramConstants.AudioCoverCacheDirPath, size => { FolderSize = size; });

        ClearCacheCommand = ReactiveCommand.Create(() =>
        {
            // 清空文件夹
            var folder = ProgramConstants.AudioCoverCacheDirPath;

            // 删除所有文件
            foreach (var file in Directory.GetFiles(folder))
            {
                File.Delete(file);
            }

            // 删除所有子文件夹
            foreach (var dir in Directory.GetDirectories(folder))
            {
                Directory.Delete(dir, true);
            }
        });
    }

    // 可读字符串
    public string FolderSizeReadable =>
        BytesToReadable(FolderSize);

    // 字节 -> 可读字符串
    static string BytesToReadable(long bytes)
    {
        string[] sizes = ["B", "KB", "MB", "GB", "TB"];
        double len = bytes;
        var order = 0;
        while (len >= 1024 && order < sizes.Length - 1)
        {
            len /= 1024;
            order++;
        }

        return $"{len:0.##} {sizes[order]}";
    }
}

public class CacheSizeToStringConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return "当前缓存： " + value;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return Binding.DoNothing;
    }
}