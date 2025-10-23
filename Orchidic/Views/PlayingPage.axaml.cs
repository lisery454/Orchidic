using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace Orchidic.Views;

public partial class PlayingPage : UserControl
{
    public PlayingPage()
    {
        InitializeComponent();
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
        return Math.Min(500, Math.Min(width, height) * 0.6);
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