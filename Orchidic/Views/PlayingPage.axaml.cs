using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Styling;
using SkiaSharp;

namespace Orchidic.Views;

public partial class PlayingPage : UserControl
{
    private readonly Bitmap _bitmap;

    public Bitmap Bitmap => _bitmap;

    private CancellationTokenSource _imageFadeInCts = new();

    public PlayingPage()
    {
        var uri = new Uri("avares://Orchidic/Assets/test.png");
        using var stream = AssetLoader.Open(uri);
        _bitmap = new Bitmap(stream);
        InitializeComponent();

        AttachedToVisualTree += (_, _) => AddBlurBackground();
    }

    private async void AddBlurBackground()
    {
        // 背景图片
        var backgroundImage = new Image
        {
            Source = await CreateBlurredBitmapAsync(_bitmap, 400),
            Stretch = Stretch.UniformToFill,
            Height = 200,
            Width = 200,
            Opacity = 0
        };

        // 创建绑定
        var multiBinding = new MultiBinding
        {
            Converter = new CoverBgSizeConverter(),
            Bindings =
            {
                new Binding
                {
                    Path = "Bounds.Width",
                    Source = CoverParentBg,
                    Mode = BindingMode.OneWay
                },
                new Binding
                {
                    Path = "Bounds.Height",
                    Source = CoverParentBg,
                    Mode = BindingMode.OneWay
                }
            }
        };
        backgroundImage.Bind(Image.WidthProperty, multiBinding);
        backgroundImage.Bind(Image.HeightProperty, multiBinding);

        // 插入到背景层
        if (CoverParentBg.Children.Count > 0)
            CoverParentBg.Children.Insert(0, backgroundImage);
        else
            CoverParentBg.Children.Add(backgroundImage);

        // 🔥 添加淡入动画
        var fadeIn = new Animation
        {
            Duration = TimeSpan.FromMilliseconds(500), // 持续时间
            Easing = new QuadraticEaseInOut(),
            FillMode = FillMode.Forward,
            Children =
            {
                new KeyFrame
                {
                    Cue = new Cue(0d),
                    Setters = { new Setter(OpacityProperty, 0.0) }
                },
                new KeyFrame
                {
                    Cue = new Cue(1d),
                    Setters = { new Setter(OpacityProperty, 0.6) }
                }
            }
        };

        _imageFadeInCts = new CancellationTokenSource();
        await fadeIn.RunAsync(backgroundImage, _imageFadeInCts.Token);
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
        Console.WriteLine(stopwatch.ElapsedMilliseconds);

        return new Bitmap(outputStream);
    }

    private static async Task<Bitmap> CreateBlurredBitmapAsync(Bitmap source, float blurRadius)
    {
        return await Task.Run(() => CreateBlurredBitmap(source, blurRadius));
    }
}

public class CoverBgSizeConverter : IMultiValueConverter
{
    public object Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count != 2) return 0;

        var width = (double)(values[0] ?? 320);
        var height = (double)(values[1] ?? 320);
        return Math.Min(500, Math.Min(width, height) * 0.6) + 400;
    }
}

public class CoverSizeConverter : IMultiValueConverter
{
    public object Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count != 2) return 0;

        var width = (double)(values[0] ?? 320);
        var height = (double)(values[1] ?? 320);
        return Math.Min(500, Math.Min(width, height) * 0.6);
    }
}

public class CoverCornerRadiusConverter : IMultiValueConverter
{
    public object Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count != 2) return 0;

        var width = (double)(values[0] ?? 320);
        var height = (double)(values[1] ?? 320);
        return Math.Min(500, Math.Min(width, height) * 0.6 * 0.5);
    }
}

public class BlurRadiusConverter : IMultiValueConverter
{
    public object Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count != 2) return 0;

        var width = (double)(values[0] ?? 320);
        var height = (double)(values[1] ?? 320);
        return Math.Min(width, height) * 0.6 / 300 * 200;
    }
}

public class PanelHeightConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return (double)value! * 0.33;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return BindingOperations.DoNothing;
    }
}