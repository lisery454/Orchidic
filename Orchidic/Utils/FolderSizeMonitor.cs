namespace Orchidic.Utils;

public class FolderSizeMonitor
{
    private readonly string _folder;
    private readonly Action<long> _setFunc;

    public FolderSizeMonitor(string folder, Action<long> setFunc)
    {
        _folder = folder;
        _setFunc = setFunc;

        setFunc(ComputeFolderSize(folder));

        var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
        timer.Tick += async (_, _) => { await Task.Run(CalculateAsync); };
        timer.Start();
    }

    private int _pending;

    private async void CalculateAsync()
    {
        // 防抖：短时间内多个事件只算一次
        Interlocked.Exchange(ref _pending, 1);
        await Task.Delay(500);

        if (Interlocked.Exchange(ref _pending, 0) == 1)
        {
            var size = await Task.Run(() => ComputeFolderSize(_folder));
            _setFunc.Invoke(size);
        }
    }

    private static long ComputeFolderSize(string folder)
    {
        long size = 0;

        try
        {
            foreach (var file in Directory.EnumerateFiles(folder, "*", SearchOption.AllDirectories))
            {
                try
                {
                    size += new FileInfo(file).Length;
                }
                catch
                {
                    // ignored
                }
            }
        }
        catch
        {
            // ignored
        }

        return size;
    }
}