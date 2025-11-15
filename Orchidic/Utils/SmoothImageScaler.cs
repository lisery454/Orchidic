namespace Orchidic.Utils;

public static class SmoothImageScaler
{
    /// <summary>
    /// 从原图中取中心正方形 → 缩放 → 高斯模糊 → 减少色带
    /// </summary>
    public static Task<BitmapSource> ScaleWithSmoothBlurAsync(BitmapSource source, int targetSize = 200,
        double blurRadius = 1.5)
    {
        // 在后台线程执行
        return Task.Run(() => ScaleInternal(source, targetSize, blurRadius));
    }

    private static BitmapSource ScaleInternal(BitmapSource source, int targetSize, double blurRadius)
    {
        if (!source.IsFrozen && source.CanFreeze)
            source.Freeze();

        int srcW = source.PixelWidth;
        int srcH = source.PixelHeight;
        int bpp = (source.Format.BitsPerPixel + 7) / 8;
        int stride = srcW * bpp;

        byte[] srcBuffer = new byte[stride * srcH];
        source.CopyPixels(srcBuffer, stride, 0);

        // 裁剪中心正方形
        int side = Math.Min(srcW, srcH);
        int offsetX = (srcW - side) / 2;
        int offsetY = (srcH - side) / 2;
        byte[] cropped = new byte[side * side * bpp];

        for (int y = 0; y < side; y++)
        {
            Array.Copy(srcBuffer, ((offsetY + y) * srcW + offsetX) * bpp,
                cropped, y * side * bpp, side * bpp);
        }

        // 缩放
        byte[] scaled = ResizeNearestNeighbor(cropped, side, side, targetSize, targetSize, bpp);

        // 高斯模糊
        byte[] blurred = GaussianBlur(scaled, targetSize, targetSize, bpp, blurRadius);

        // 减少色带
        var rand = new Random();
        for (int i = 0; i < blurred.Length; i++)
        {
            int noise = rand.Next(-2, 3);
            blurred[i] = (byte)Math.Clamp(blurred[i] + noise, 0, 255);
        }

        // 写入 WriteableBitmap
        var wb = new WriteableBitmap(targetSize, targetSize, 96, 96, PixelFormats.Pbgra32, null);
        wb.WritePixels(new Int32Rect(0, 0, targetSize, targetSize), blurred, targetSize * bpp, 0);
        wb.Freeze();

        return wb;
    }

    private static byte[] ResizeNearestNeighbor(byte[] src, int srcW, int srcH, int dstW, int dstH, int bpp)
    {
        byte[] dst = new byte[dstW * dstH * bpp];
        for (int y = 0; y < dstH; y++)
        {
            int srcY = y * srcH / dstH;
            for (int x = 0; x < dstW; x++)
            {
                int srcX = x * srcW / dstW;
                Array.Copy(src, (srcY * srcW + srcX) * bpp, dst, (y * dstW + x) * bpp, bpp);
            }
        }

        return dst;
    }

    private static byte[] GaussianBlur(byte[] src, int width, int height, int bpp, double radius)
    {
        if (radius < 0.5) return src; // 半径过小，直接返回

        int kernelSize = (int)Math.Ceiling(radius) * 2 + 1;
        double sigma = radius / 3;
        double[] kernel = new double[kernelSize];
        double sum = 0;
        int half = kernelSize / 2;

        // 构造一维高斯核
        for (int i = 0; i < kernelSize; i++)
        {
            int x = i - half;
            kernel[i] = Math.Exp(-0.5 * (x * x) / (sigma * sigma));
            sum += kernel[i];
        }

        for (int i = 0; i < kernelSize; i++)
            kernel[i] /= sum;

        // 横向模糊
        byte[] tmp = new byte[src.Length];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                for (int c = 0; c < bpp; c++)
                {
                    double val = 0;
                    for (int k = -half; k <= half; k++)
                    {
                        int idx = x + k;
                        if (idx < 0) idx = 0;
                        if (idx >= width) idx = width - 1;
                        val += src[(y * width + idx) * bpp + c] * kernel[k + half];
                    }

                    tmp[(y * width + x) * bpp + c] = (byte)Math.Clamp((int)(val + 0.5), 0, 255);
                }
            }
        }

        // 纵向模糊
        byte[] dst = new byte[src.Length];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                for (int c = 0; c < bpp; c++)
                {
                    double val = 0;
                    for (int k = -half; k <= half; k++)
                    {
                        int idx = y + k;
                        if (idx < 0) idx = 0;
                        if (idx >= height) idx = height - 1;
                        val += tmp[(idx * width + x) * bpp + c] * kernel[k + half];
                    }

                    dst[(y * width + x) * bpp + c] = (byte)Math.Clamp((int)(val + 0.5), 0, 255);
                }
            }
        }

        return dst;
    }
}