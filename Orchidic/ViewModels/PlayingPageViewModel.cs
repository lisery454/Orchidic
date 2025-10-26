using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Orchidic.Utils;
using ReactiveUI;
using SkiaSharp;

namespace Orchidic.ViewModels;

public class PlayingPageViewModel : ViewModelBase
{
    public Bitmap Bitmap { get; }

    public Task<Bitmap> BlurredBitmap { get; }

    private TimeSpan _totalTime;

    public TimeSpan TotalTime
    {
        get => _totalTime;
        set => this.RaiseAndSetIfChanged(ref _totalTime, value);
    }

    private TimeSpan _currentTime;

    public TimeSpan CurrentTime
    {
        get => _currentTime;
        set => this.RaiseAndSetIfChanged(ref _currentTime, value);
    }

    private double _progress;

    public double Progress
    {
        get => _progress;
        set
        {
            this.RaiseAndSetIfChanged(ref _progress, Math.Clamp(value, 0, 1));
            CurrentTime = new TimeSpan(0, 0, 0, (int)(_progress * TotalTime.TotalSeconds));
        }
    }

    public PlayingPageViewModel()
    {
        var uri = new Uri("avares://Orchidic/Assets/test.png");
        using var stream = AssetLoader.Open(uri);
        Bitmap = new Bitmap(stream);
        BlurredBitmap = CreateBlurredBitmapAsync(Bitmap, 400);
        CurrentTime = TimeSpan.FromMinutes(1.4);
        TotalTime = TimeSpan.FromMinutes(3.4);
        Progress = CurrentTime.TotalSeconds / TotalTime.TotalSeconds;
    }

    private static Bitmap CreateBlurredBitmap(Bitmap source, float blurRadius)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        // 1. Avalonia Bitmap -> SKBitmap
        using var stream = new MemoryStream();
        source.Save(stream);
        stream.Position = 0;
        using var skBitmap = SKBitmap.Decode(stream);

        var width = skBitmap.Width;
        // var height = skBitmap.Height; // width应该和height是一致的
        var r = (int)Math.Ceiling(blurRadius); // 向上取整

        const int scaledSize = 10;
        var scaledBlurRadius = r * scaledSize / width;

        var scaledBitmap = new SKBitmap(scaledSize, scaledSize);
        skBitmap.ScalePixels(scaledBitmap, SKFilterQuality.Low);

        // 2. 构建“圆形卷积核”
        var kernelRadius = Math.Max(1, scaledBlurRadius);
        var size = kernelRadius * 2 + 1;
        var kernel = new float[size, size];
        var sum = 0f;

        for (var y = 0; y < size; y++)
        {
            for (var x = 0; x < size; x++)
            {
                float dx = x - kernelRadius;
                float dy = y - kernelRadius;
                if (!(Math.Sqrt(dx * dx + dy * dy) <= kernelRadius)) continue;
                kernel[y, x] = 1f;
                sum += 1f;
            }
        }

        // 归一化
        for (var y = 0; y < size; y++)
        {
            for (var x = 0; x < size; x++)
            {
                kernel[y, x] /= sum;
            }
        }

        var kernelFlat = kernel.Cast<float>().ToArray();

        // 3. 创建卷积滤镜
        var filter = SKImageFilter.CreateMatrixConvolution(
            new SKSizeI(size, size),
            kernelFlat,
            1.0f, // scale
            0.0f, // bias
            new SKPointI(kernelRadius, kernelRadius),
            SKShaderTileMode.Clamp,
            true, // convolveAlpha
            null // input
        );

        // 4. 创建输出 Surface
        using var surface =
            SKSurface.Create(new SKImageInfo(scaledSize + 2 * scaledBlurRadius, scaledSize + 2 * scaledBlurRadius));
        var canvas = surface.Canvas;
        canvas.Clear(SKColors.Transparent);

        using var paint = new SKPaint();
        paint.ImageFilter = filter;

        // 5. 绘制到中心
        canvas.DrawBitmap(scaledBitmap, scaledBlurRadius, scaledBlurRadius, paint);
        canvas.Flush();

        // 6. 输出 Avalonia Bitmap
        using var image = surface.Snapshot();
        using var imageData = image.Encode(SKEncodedImageFormat.Png, 0);
        using var outputStream = new MemoryStream();
        imageData.SaveTo(outputStream);
        outputStream.Position = 0;

        stopwatch.Stop();

        return new Bitmap(outputStream);
    }

    private static async Task<Bitmap> CreateBlurredBitmapAsync(Bitmap source, float blurRadius)
    {
        return await Task.Run(() => CreateBlurredBitmap(source, blurRadius));
    }
}