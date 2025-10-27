using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Avalonia.Media.Imaging;
using SkiaSharp;

namespace Orchidic.Utils;

public static class BlurBitmapHelper
{
    public static Bitmap CreateBlurredBitmap(this Bitmap source, float blurRadius)
    {
        // 1. Avalonia Bitmap -> SKBitmap
        using var squareBitMap = ToSquareBitmap(source);
        using var stream = new MemoryStream();
        squareBitMap.Save(stream);
        stream.Position = 0;
        using var skBitmap = SKBitmap.Decode(stream);

        var width = skBitmap.Width;
        
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


        return new Bitmap(outputStream);
    }

    public static Bitmap ToSquareBitmap(Bitmap source)
    {
        using var stream = new MemoryStream();
        source.Save(stream);
        stream.Position = 0;

        using var skBitmap = SKBitmap.Decode(stream);

        int width = skBitmap.Width;
        int height = skBitmap.Height;
        int size = Math.Min(width, height);

        int offsetX = (width - size) / 2;
        int offsetY = (height - size) / 2;

        var square = new SKBitmap(size, size);
        using (var canvas = new SKCanvas(square))
        {
            canvas.Clear(SKColors.Transparent);
            canvas.DrawBitmap(skBitmap,
                new SKRect(offsetX, offsetY, offsetX + size, offsetY + size),
                new SKRect(0, 0, size, size));
        }

        // 输出 Avalonia Bitmap
        using var image = SKImage.FromBitmap(square);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        using var outputStream = new MemoryStream();
        data.SaveTo(outputStream);
        outputStream.Position = 0;

        return new Bitmap(outputStream);
    }
}