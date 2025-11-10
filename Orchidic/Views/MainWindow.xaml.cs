using Orchidic.Utils;
using Orchidic.ViewModels;

namespace Orchidic.Views;

// public partial class MainWindow : IViewFor<MainWindowViewModel>
// {
//     object? IViewFor.ViewModel
//     {
//         get => ViewModel;
//         set => ViewModel = (MainWindowViewModel)value!;
//     }
//
//     public MainWindowViewModel? ViewModel { get; set; }
// }

public partial class MainWindow
{
    public MainWindow()
    {
        Loaded += Window_Loaded;
        WindowCornerRestorer.ApplyRoundCorner(this);
        DataContext = App.Current.Services.GetService<MainWindowViewModel>();
        InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        WindowAnimRestorer.AddAnimTo(this);
    }

    private void ToggleMaximize()
    {
        WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
    }

    private void MinButton_OnClick(object? sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void MaxButton_OnClick(object? sender, RoutedEventArgs e)
    {
        ToggleMaximize();
    }

    private void CloseButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}

public class MenuWidthConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return Math.Min(Math.Max((double)value! * 0.2, 244), 320);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return Binding.DoNothing;
    }
}

public class BlurCoverSizeConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length != 2) return 0;

        var width = (double)values[0];
        var height = (double)values[1];
        var result = Math.Max(width, height) * 1.1;
        return result;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        return [];
    }
}

public static class ImageFadeExtensions
{
    public static readonly DependencyProperty AnimatedSourceProperty =
        DependencyProperty.RegisterAttached(
            "AnimatedSource",
            typeof(BitmapSource),
            typeof(ImageFadeExtensions),
            new PropertyMetadata(null, OnAnimatedSourceChanged));

    public static void SetAnimatedSource(Image image, BitmapSource value) =>
        image.SetValue(AnimatedSourceProperty, value);

    public static BitmapSource GetAnimatedSource(Image image) =>
        (BitmapSource)image.GetValue(AnimatedSourceProperty);

    private static readonly Dictionary<Image, Image> _fadeImages = new();
    private static readonly Dictionary<Image, Storyboard> _runningStoryboards = new();
    private static double opacity = -1;

    private static void OnAnimatedSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not Image img) return;
        if (e.NewValue is not BitmapSource newBitmap) return;
        // 父容器必须是 Grid
        if (img.Parent is not Grid container)
        {
            throw new InvalidOperationException("Image parent must be a Grid for AnimatedSource.");
        }

        if (!double.IsNaN(img.ActualWidth) && !double.IsNaN(img.ActualHeight) && img.ActualWidth > 0 &&
            img.ActualHeight > 0)
        {
            startAnimation();
        }
        else
        {
            // 尺寸未确定，等待 Layout 完成再执行
            void handler(object s, SizeChangedEventArgs args)
            {
                Console.WriteLine(img.ActualWidth);
                container.SizeChanged -= handler;
                startAnimation();
            }

            container.SizeChanged += handler;
        }

        return;

        void startAnimation()
        {
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

            // 创建一个临时 Image 用于淡入
            var fadeImage = new Image
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
                Opacity = 0
            };

            container.Children.Add(fadeImage);
            _fadeImages[img] = fadeImage;

            if (opacity < 0) opacity = img.Opacity;

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

            var fadeIn = new DoubleAnimation
            {
                From = 0,
                To = opacity,
                Duration = TimeSpan.FromMilliseconds(300),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };
            Storyboard.SetTarget(fadeIn, fadeImage);
            Storyboard.SetTargetProperty(fadeIn, new PropertyPath(UIElement.OpacityProperty));
            sb.Children.Add(fadeIn);

            sb.Completed += (_, _) =>
            {
                img.BeginAnimation(UIElement.OpacityProperty, null);
                fadeImage.BeginAnimation(UIElement.OpacityProperty, null);

                img.Source = newBitmap;
                img.Opacity = opacity;

                container.Children.Remove(fadeImage);
                _fadeImages.Remove(img);
                _runningStoryboards.Remove(img);
            };

            _runningStoryboards[img] = sb;
            sb.Begin();
        }
    }
}