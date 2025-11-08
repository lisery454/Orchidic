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
                using var mem = new MemoryStream(pic.Data.Data);
                // 使用 BitmapImage 从 MemoryStream 创建 WPF 可用的 BitmapSource
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad; // 立即加载，释放流安全
                bitmap.StreamSource = mem;
                bitmap.EndInit();
                bitmap.Freeze(); // 可跨线程使用
                return bitmap;
            }
        }
        catch
        {
            ;
        }

        return GetDefaultCover();
    }

    public BitmapSource GetDefaultCover()
    {
        const int height = 100;
        const int width = 100;
        var rtb = new RenderTargetBitmap(width, height, 96, 96, System.Windows.Media.PixelFormats.Pbgra32);

        // 使用 DrawingVisual 绘制内容
        var dv = new System.Windows.Media.DrawingVisual();
        var brush = Application.Current.Resources["SecondaryBrush"] as System.Windows.Media.SolidColorBrush;
        using (var dc = dv.RenderOpen())
        {
            // 填充矩形
            dc.DrawRectangle(brush, null, new Rect(0, 0, width, height));
        }

        // 渲染到 RenderTargetBitmap
        rtb.Render(dv);
        return rtb;
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