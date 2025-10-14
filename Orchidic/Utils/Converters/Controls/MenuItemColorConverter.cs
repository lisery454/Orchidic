using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace Orchidic.Utils.Converters;

public class MenuItemColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is true
            ? Color.Parse("#465a5f")
            : Color.Parse("#9ea8aa");
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}