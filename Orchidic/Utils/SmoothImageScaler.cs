using System.Windows.Shapes;

namespace Orchidic.Utils;

public static class SmoothImageScaler
{
    /// <summary>
    /// 从原图中取中心正方形 → 缩放到指定大小 → 模糊 → 色彩平滑
    /// </summary>
    public static BitmapSource ScaleWithSmoothBlur(BitmapSource source, double targetSize = 200,
        double blurRadius = 1.5)
    {
        // 🌟 第一步：规范 DPI 并裁剪为中心正方形
        var normalized = NormalizeDpi(source);
        var cropped = CropCenterSquare(normalized);

        // 🌟 第二步：缩放成固定大小（例如200×200）
        var scaled = ResizeToFixedSize(cropped, (int)targetSize, (int)targetSize);

        // 🌟 第三步：应用平滑模糊
        var blurred = ApplySmoothBlur(scaled, blurRadius);

        // 🌟 第四步：减少色带
        var final = ReduceColorBanding(blurred);

        return final;
    }

    /// <summary>
    /// 统一 DPI 到 96，防止缩放不一致
    /// </summary>
    private static BitmapSource NormalizeDpi(BitmapSource source, double dpiX = 96, double dpiY = 96)
    {
        if (Math.Abs(source.DpiX - dpiX) < 0.1 && Math.Abs(source.DpiY - dpiY) < 0.1)
            return source;

        var scale = new ScaleTransform(dpiX / source.DpiX, dpiY / source.DpiY);
        var transformed = new TransformedBitmap(source, scale);
        transformed.Freeze();
        return transformed;
    }

    /// <summary>
    /// 裁剪出图像中心的正方形部分
    /// </summary>
    private static BitmapSource CropCenterSquare(BitmapSource source)
    {
        if (!source.IsFrozen && source.CanFreeze)
            source.Freeze();

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

    /// <summary>
    /// 高质量缩放到固定大小
    /// </summary>
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

    /// <summary>
    /// 使用高质量 Gaussian 模糊
    /// </summary>
    private static BitmapSource ApplySmoothBlur(BitmapSource source, double radius)
    {
        var blur = new BlurEffect
        {
            Radius = radius,
            KernelType = KernelType.Gaussian,
            RenderingBias = RenderingBias.Quality
        };

        var rect = new Rectangle
        {
            Width = source.PixelWidth,
            Height = source.PixelHeight,
            Fill = new ImageBrush(source),
            Effect = blur
        };

        rect.Arrange(new Rect(0, 0, source.PixelWidth, source.PixelHeight));

        var renderTarget = new RenderTargetBitmap(
            source.PixelWidth, source.PixelHeight,
            source.DpiX, source.DpiY, PixelFormats.Pbgra32);

        renderTarget.Render(rect);
        renderTarget.Freeze();

        return renderTarget;
    }

    /// <summary>
    /// 添加轻微噪点以减少色带
    /// </summary>
    private static BitmapSource ReduceColorBanding(BitmapSource source)
    {
        var writeableBitmap = new WriteableBitmap(source);

        Random rand = new Random();
        int bytesPerPixel = (source.Format.BitsPerPixel + 7) / 8;
        int stride = writeableBitmap.PixelWidth * bytesPerPixel;

        byte[] pixels = new byte[writeableBitmap.PixelHeight * stride];
        writeableBitmap.CopyPixels(pixels, stride, 0);

        for (int i = 0; i < pixels.Length; i++)
        {
            int noise = rand.Next(-2, 3);
            int newValue = pixels[i] + noise;
            pixels[i] = (byte)Math.Max(0, Math.Min(255, newValue));
        }

        writeableBitmap.WritePixels(
            new Int32Rect(0, 0, writeableBitmap.PixelWidth, writeableBitmap.PixelHeight),
            pixels, stride, 0);

        writeableBitmap.Freeze();
        return writeableBitmap;
    }
}