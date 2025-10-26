using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Styling;
using Orchidic.ViewModels;

namespace Orchidic.Views;

public partial class PlayingPage : UserControl
{
    private CancellationTokenSource _imageFadeInCts = new();

    private double ProgressBarWidth { get; set; }

    public PlayingPage()
    {
        InitializeComponent();

        AttachedToVisualTree += (_, _) => AddBlurBackground();

        ProgressBarBg.GetObservable(BoundsProperty)
            .Subscribe(bounds => { ProgressBarWidth = bounds.Width; });
    }

    private async void AddBlurBackground()
    {
        var model = DataContext as PlayingPageViewModel;
        // 背景图片
        var backgroundImage = new Image
        {
            Source = await model!.BlurredCover,
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
        backgroundImage.Bind(WidthProperty, multiBinding);
        backgroundImage.Bind(HeightProperty, multiBinding);

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

    private bool _isDragging;

    private void ProgressBar_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            _isDragging = true;

            (DataContext as PlayingPageViewModel)!.Progress = e.GetPosition(sender as Border).X / ProgressBarWidth;
            e.Pointer.Capture((IInputElement)sender!); // 捕获鼠标
        }
    }

    private void ProgressBar_OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (_isDragging)
        {
            _isDragging = false;
            e.Pointer.Capture(null); // 释放捕获
        }
    }

    private void ProgressBar_OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (_isDragging)
        {
            (DataContext as PlayingPageViewModel)!.Progress = e.GetPosition(sender as Border).X / ProgressBarWidth;
        }
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
        return Math.Min(500, Math.Min(width, height) * 0.6 * 0.2);
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

public class TimeSpanToStringConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not TimeSpan timeSpan)
            return "";

        var format = parameter as string ?? @"mm\:ss";
        if (timeSpan.TotalHours > 1)
        {
            format = parameter as string ?? @"hh\:mm\:ss";
        }

        return timeSpan.ToString(format);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return BindingOperations.DoNothing;
    }
}

public class ProgressToWidthConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count < 2)
            return 0;

        if (values[0] is not double totalWidth || double.IsNaN(totalWidth))
            return 0;

        if (values[1] is not double progress)
            return 0;

        // 进度一般在 [0,1]
        progress = Math.Clamp(progress, 0, 1);
        return totalWidth * progress;
    }
}