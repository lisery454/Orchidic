namespace Orchidic.Utils;

public class ImageFadeBehavior : Behavior<Image>
{
    public static readonly DependencyProperty SourceProperty =
        DependencyProperty.Register(
            nameof(Source),
            typeof(BitmapSource),
            typeof(ImageFadeBehavior),
            new PropertyMetadata(null, OnSourceChanged));

    public BitmapSource Source
    {
        get => (BitmapSource)GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is BitmapSource bmp)
            ((ImageFadeBehavior)d).StartFade(bmp);
    }

    private readonly Dictionary<Image, Image> _fadeImages = new();
    private readonly Dictionary<Image, Storyboard> _runningStoryboards = new();
    private double opacity = -1;

    private void StartFade(BitmapSource newBitmap)
    {
        if (AssociatedObject is not { } img) return;
        // 父容器必须是 Grid
        if (img.Parent is not Grid container)
        {
            throw new InvalidOperationException("Image parent must be a Grid for AnimatedSource.");
        }

        startAnimation();

        return;

        void startAnimation()
        {
            const double fadeSize = 1.4;
            // 如果上一次有动画，取消它
            if (_runningStoryboards.TryGetValue(img, out var oldStoryboard))
            {
                oldStoryboard.Stop();
            }

            // 移除旧的临时 Image
            if (_fadeImages.TryGetValue(img, out var oldFadeImage))
            {
                container.Children.Remove(oldFadeImage);
                _fadeImages.Remove(img);
            }

            EnsureTransform(img);


            // 创建一个临时 Image 用于淡入
            var newFadeImage = new Image
            {
                Source = newBitmap,
                Stretch = img.Stretch,
                SnapsToDevicePixels = img.SnapsToDevicePixels,
                UseLayoutRounding = img.UseLayoutRounding,
                ClipToBounds = img.ClipToBounds,
                HorizontalAlignment = img.HorizontalAlignment,
                VerticalAlignment = img.VerticalAlignment,
                Width = img.Width,
                Height = img.Height,
                Opacity = 0,
                RenderTransform = new ScaleTransform(fadeSize, fadeSize),
                RenderTransformOrigin = new Point(0.5, 0.5)
            };


            container.Children.Add(newFadeImage);
            _fadeImages[img] = newFadeImage;

            if (opacity < 0) opacity = img.Opacity;

            // old
            var sb = new Storyboard();
            var fadeOut = new DoubleAnimation
            {
                From = opacity,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut },
            };
            Storyboard.SetTarget(fadeOut, img);
            Storyboard.SetTargetProperty(fadeOut, new PropertyPath(UIElement.OpacityProperty));
            sb.Children.Add(fadeOut);
            var oldScaleX = new DoubleAnimation(1.0, fadeSize, TimeSpan.FromMilliseconds(300))
            {
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };
            Storyboard.SetTarget(oldScaleX, img);
            Storyboard.SetTargetProperty(oldScaleX, new PropertyPath("RenderTransform.ScaleX"));
            sb.Children.Add(oldScaleX);
            var oldScaleY = new DoubleAnimation(1.0, fadeSize, TimeSpan.FromMilliseconds(300))
            {
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };
            Storyboard.SetTarget(oldScaleY, img);
            Storyboard.SetTargetProperty(oldScaleY, new PropertyPath("RenderTransform.ScaleY"));
            sb.Children.Add(oldScaleY);

            // new
            var fadeIn = new DoubleAnimation
            {
                From = 0,
                To = opacity,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };
            Storyboard.SetTarget(fadeIn, newFadeImage);
            Storyboard.SetTargetProperty(fadeIn, new PropertyPath(UIElement.OpacityProperty));
            sb.Children.Add(fadeIn);
            var newScaleX = new DoubleAnimation(fadeSize, 1.0, TimeSpan.FromMilliseconds(300))
            {
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };
            Storyboard.SetTarget(newScaleX, newFadeImage);
            Storyboard.SetTargetProperty(newScaleX, new PropertyPath("RenderTransform.ScaleX"));
            sb.Children.Add(newScaleX);
            var newScaleY = new DoubleAnimation(fadeSize, 1.0, TimeSpan.FromMilliseconds(300))
            {
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };
            Storyboard.SetTarget(newScaleY, newFadeImage);
            Storyboard.SetTargetProperty(newScaleY, new PropertyPath("RenderTransform.ScaleY"));
            sb.Children.Add(newScaleY);

            // end
            sb.Completed += (_, _) =>
            {
                img.BeginAnimation(UIElement.OpacityProperty, null);
                newFadeImage.BeginAnimation(UIElement.OpacityProperty, null);

                img.RenderTransform = new ScaleTransform(1.0, 1.0);
                img.Source = newBitmap;
                img.Opacity = opacity;

                container.Children.Remove(newFadeImage);
                _fadeImages.Remove(img);
                _runningStoryboards.Remove(img);
            };

            // run
            _runningStoryboards[img] = sb;
            sb.Begin();
        }
    }

    private static void EnsureTransform(Image img)
    {
        if (img.RenderTransform is not ScaleTransform)
        {
            img.RenderTransformOrigin = new Point(0.5, 0.5);
            img.RenderTransform = new ScaleTransform(1.0, 1.0);
        }
    }
}