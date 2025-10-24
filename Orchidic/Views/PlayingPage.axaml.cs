using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using SkiaSharp;

namespace Orchidic.Views;

public partial class PlayingPage : UserControl
{
    private readonly Bitmap _bitmap;

    public Bitmap Bitmap => _bitmap;

    public PlayingPage()
    {
        var uri = new Uri("avares://Orchidic/Assets/test.png");
        using var stream = AssetLoader.Open(uri);
        _bitmap = new Bitmap(stream);
        InitializeComponent();

        AttachedToVisualTree += (_, __) => AddBlurBackground();
    }

    private void AddBlurBackground()
    {
        // 背景图片
        var backgroundImage = new Image
        {
            Source = CreateBlurredBitmap(_bitmap, 200),
            Stretch = Stretch.UniformToFill,
            Height = 200,
            Width = 200,
            Opacity = 0.5
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
    }

    private static Bitmap CreateBlurredBitmap(Bitmap source, float blurRadius)
    {
        // 1. Avalonia Bitmap -> SKBitmap
        using var stream = new MemoryStream();
        source.Save(stream);
        stream.Position = 0;
        using var skBitmap = SKBitmap.Decode(stream);

        int W = skBitmap.Width;
        int H = skBitmap.Height;
        int r = (int)Math.Ceiling(blurRadius); // 向上取整，保证空间足够

        // 2. 创建输出 Surface，比原图大 2*r，以容纳模糊边缘
        using var surface = SKSurface.Create(new SKImageInfo(W + 2 * r, H + 2 * r));
        var canvas = surface.Canvas;
        canvas.Clear(SKColors.Transparent);

        // 3. 设置模糊滤镜
        using var paint = new SKPaint
        {
            ImageFilter = SKImageFilter.CreateBlur(blurRadius, blurRadius)
        };

        // 4. 绘制原图到 Surface 中心
        canvas.DrawBitmap(skBitmap, r, r, paint);
        canvas.Flush();

        // 5. 输出 Avalonia Bitmap
        using var image = surface.Snapshot();
        using var imageData = image.Encode(SKEncodedImageFormat.Png, 100);
        using var outputStream = new MemoryStream();
        imageData.SaveTo(outputStream);
        outputStream.Position = 0;

        return new Bitmap(outputStream);
    }

    private static Bitmap CreateBallBlurredBitmap(Bitmap source, float blurRadius, float scaleFactor = 0.25f)
    {
        // 1. Avalonia Bitmap -> SKBitmap
        using var stream = new MemoryStream();
        source.Save(stream);
        stream.Position = 0;
        using var skBitmapOriginal = SKBitmap.Decode(stream);

        int W = skBitmapOriginal.Width;
        int H = skBitmapOriginal.Height;
        int r = (int)Math.Ceiling(blurRadius);

        // 2. 缩小 Bitmap，降低计算量
        int smallW = Math.Max(1, (int)(W * scaleFactor));
        int smallH = Math.Max(1, (int)(H * scaleFactor));

        using var skBitmapSmall = skBitmapOriginal.Resize(new SKImageInfo(smallW, smallH), SKFilterQuality.Medium);

        // 3. 创建输出 Surface，比原图大 2*r，以容纳模糊边缘
        using var surface = SKSurface.Create(new SKImageInfo(W + 2 * r, H + 2 * r));
        var canvas = surface.Canvas;
        canvas.Clear(SKColors.Transparent);

        // 4. Morphology 膨胀 + 高斯模糊
        using (var paint = new SKPaint
               {
                   // 注意：半径也要缩放
                   ImageFilter = SKImageFilter.CreateDilate(r * scaleFactor, r * scaleFactor)
               })
        {
            // 把缩小后的图片放到输出 surface 中间
            float scaleX = (float)W / smallW;
            float scaleY = (float)H / smallH;
            var destRect = new SKRect(r, r, r + smallW * scaleX, r + smallH * scaleY);
            canvas.DrawBitmap(skBitmapSmall, destRect, paint);
        }

        using (var paintBlur = new SKPaint
               {
                   ImageFilter = SKImageFilter.CreateBlur(blurRadius / 2f, blurRadius / 2f)
               })
        {
            float scaleX = (float)W / smallW;
            float scaleY = (float)H / smallH;
            var destRect = new SKRect(r, r, r + smallW * scaleX, r + smallH * scaleY);
            canvas.DrawBitmap(skBitmapSmall, destRect, paintBlur);
        }

        canvas.Flush();

        // 5. 输出 Avalonia Bitmap
        using var image = surface.Snapshot();
        using var imageData = image.Encode(SKEncodedImageFormat.Png, 100);
        using var outputStream = new MemoryStream();
        imageData.SaveTo(outputStream);
        outputStream.Position = 0;

        return new Bitmap(outputStream);
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