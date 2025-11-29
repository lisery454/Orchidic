using Orchidic.Models;

namespace Orchidic.Converters;

public class PageIconConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is PageType pageType)
        {
            return pageType switch
            {
                PageType.Playing => Application.Current.Resources["MusicIconGeometry"] as Geometry,
                PageType.Queue => Application.Current.Resources["QueueIconGeometry"] as Geometry,
                PageType.List => Application.Current.Resources["ListIconGeometry"] as Geometry,
                PageType.Statistics => Application.Current.Resources["StatisticsIconGeometry"] as Geometry,
                PageType.Tools => Application.Current.Resources["ToolIconGeometry"] as Geometry,
                PageType.Settings => Application.Current.Resources["SettingIconGeometry"] as Geometry,
                _ => "default"
            };
        }

        return Binding.DoNothing;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return Binding.DoNothing;
    }
}