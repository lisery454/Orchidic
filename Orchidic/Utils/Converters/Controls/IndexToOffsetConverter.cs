using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Orchidic.Utils.Converters;

public class IndexToOffsetConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int index)
            return index * 60.0; // 每项高度 60
        return 0.0;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}