namespace Orchidic.Converters;

public class ConditionConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        var condition = values[0] as bool? ?? false;
        var obj1 = values[1];
        var obj2 = values[2];

        return condition ? obj1 : obj2;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        return [];
    }
}