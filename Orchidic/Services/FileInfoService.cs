using Orchidic.Services.Interfaces;

namespace Orchidic.Services;

public class FileInfoService : IFileInfoService
{
    public BitmapSource GetCoverFromAudio(string path)
    {
        try
        {
            using var file = TagLib.File.Create(path);
            if (file.Tag.Pictures is { Length: > 0 })
            {
                var pic = file.Tag.Pictures[0];
                var coverBitmap = LoadAlbumCoverSquare(pic.Data.Data, 400);
                return coverBitmap;
            }
        }
        catch
        {
            ;
        }

        return GetDefaultCover();
    }

    private static BitmapSource LoadAlbumCoverSquare(byte[] imageData, int targetSize = 200)
    {
        using var mem = new MemoryStream(imageData);

        // 先加载 BitmapImage，但不完全解码
        var bitmap = new BitmapImage();
        bitmap.BeginInit();
        bitmap.CacheOption = BitmapCacheOption.OnLoad; // 立即加载，MemoryStream 可释放
        bitmap.StreamSource = mem;

        // 获取原图尺寸信息
        bitmap.CreateOptions = BitmapCreateOptions.DelayCreation;
        bitmap.EndInit();

        int originalWidth = bitmap.PixelWidth;
        int originalHeight = bitmap.PixelHeight;

        // 计算等比例缩放尺寸
        // 保证最短边 >= targetSize，这样裁剪中心正方形不会放大
        double scale = (double)targetSize / Math.Min(originalWidth, originalHeight);
        int decodeWidth = (int)(originalWidth * scale);
        int decodeHeight = (int)(originalHeight * scale);

        // 重新加载 BitmapImage，限制解码大小
        mem.Position = 0; // 重置流
        var scaledBitmap = new BitmapImage();
        scaledBitmap.BeginInit();
        scaledBitmap.CacheOption = BitmapCacheOption.OnLoad;
        scaledBitmap.StreamSource = mem;
        scaledBitmap.DecodePixelWidth = decodeWidth;
        scaledBitmap.DecodePixelHeight = decodeHeight;
        scaledBitmap.EndInit();
        scaledBitmap.Freeze();

        // 裁剪中心正方形
        var cropped = CropCenterSquare(scaledBitmap);

        // 如果需要保证输出是 targetSize×targetSize，可以再缩放一次
        var resized = ResizeToFixedSize(cropped, targetSize, targetSize);

        return resized;
    }

    private static BitmapSource CropCenterSquare(BitmapSource source)
    {
        int width = source.PixelWidth;
        int height = source.PixelHeight;

        int side = Math.Min(width, height);
        int x = (width - side) / 2;
        int y = (height - side) / 2;

        var rect = new Int32Rect(x, y, side, side);
        var cropped = new CroppedBitmap(source, rect);
        cropped.Freeze();
        return cropped;
    }

    private static BitmapSource ResizeToFixedSize(BitmapSource source, int width, int height)
    {
        var visual = new DrawingVisual();
        using (var context = visual.RenderOpen())
        {
            RenderOptions.SetBitmapScalingMode(visual, BitmapScalingMode.HighQuality);
            context.DrawImage(source, new Rect(0, 0, width, height));
        }

        var renderTarget = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
        renderTarget.Render(visual);
        renderTarget.Freeze();
        return renderTarget;
    }

    public BitmapSource GetDefaultCover()
    {
        var color = (Application.Current.Resources["SecondaryBrush"] as SolidColorBrush)?.Color ?? Colors.Gray;

        return RunInSta(() =>
        {
            const int height = 100;
            const int width = 100;
            var rtb = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);

            var dv = new DrawingVisual();
            using (var dc = dv.RenderOpen())
            {
                var brush = new SolidColorBrush(color);
                dc.DrawRectangle(brush, null, new Rect(0, 0, width, height));
            }

            rtb.Render(dv);
            return rtb; // RunInSta 内部会 Freeze
        });
    }

    private static BitmapSource RunInSta(Func<BitmapSource> func)
    {
        BitmapSource? result = null;
        Exception? exception = null;

        var thread = new Thread(() =>
        {
            try
            {
                // ⚡ 这里一定要在 STA 内创建所有 WPF 对象
                result = func();

                // Freeze 在同一个 STA 线程内
                result?.Freeze();
            }
            catch (Exception ex)
            {
                exception = ex;
            }
        });

        thread.SetApartmentState(ApartmentState.STA);
        thread.IsBackground = true;
        thread.Start();
        thread.Join();

        if (exception != null)
            throw new InvalidOperationException("STA thread execution failed.", exception);

        return result!;
    }

    public string GetTitleFromAudio(string path)
    {
        try
        {
            using var file = TagLib.File.Create(path);

            // 优先读取音频标签里的标题
            var title = file.Tag.Title;

            // 如果标签里没有标题，则使用文件名
            if (string.IsNullOrWhiteSpace(title))
                title = Path.GetFileNameWithoutExtension(path);

            return title;
        }
        catch
        {
            // 如果文件无法读取或格式不支持，仍返回文件名
            return Path.GetFileNameWithoutExtension(path);
        }
    }

    public string GetDefaultTitle()
    {
        return "No Audio Playing";
    }
}