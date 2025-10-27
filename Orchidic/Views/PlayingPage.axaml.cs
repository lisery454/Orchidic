using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Input;
using Orchidic.ViewModels;

namespace Orchidic.Views;

public partial class PlayingPage : UserControl
{
    private double ProgressBarWidth { get; set; }

    public PlayingPage()
    {
        InitializeComponent();

        // AttachedToVisualTree += (_, _) => AddBlurBackground();

        ProgressBarBg.GetObservable(BoundsProperty)
            .Subscribe(bounds => { ProgressBarWidth = bounds.Width; });
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