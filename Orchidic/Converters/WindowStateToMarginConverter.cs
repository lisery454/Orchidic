namespace Orchidic.Converters;

public class WindowStateToMarginConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is WindowState state)
        {
            return state == WindowState.Maximized
                ? new Thickness(8)
                : new Thickness(0);
        }

        return new Thickness(0);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return Binding.DoNothing;
    }
}