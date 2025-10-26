using System.IO;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;

namespace Orchidic.Service;

public class FileInfoService : IFileInfoService
{
    public Bitmap GetCoverFromAudio(string path)
    {
        try
        {
            var file = TagLib.File.Create(path);
            if (file.Tag.Pictures is { Length: > 0 })
            {
                var pic = file.Tag.Pictures[0];
                using var mem = new MemoryStream(pic.Data.Data);
                // 注意：Constructing a Bitmap from a stream consumes it, but OK here inside using.
                return new Bitmap(mem);
            }
        }
        catch
        {
            ;
        }

        return GetDefaultCover();
    }

    public Bitmap GetDefaultCover()
    {
        var pixelSize = new PixelSize(100, 100);
        var dpi = new Vector(96, 96);
        var rtb = new RenderTargetBitmap(pixelSize, dpi);

        // 填充矩形
        using var ctx = rtb.CreateDrawingContext(true);
        var color = Color.Parse("#9ea8aa");
        var brush = new SolidColorBrush(color);
        ctx.FillRectangle(brush, new Rect(0, 0, 100, 100));
        return rtb;
    }
}