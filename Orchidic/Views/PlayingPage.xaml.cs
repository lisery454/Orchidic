using Orchidic.ViewModels;

namespace Orchidic.Views;

public partial class PlayingPage : IViewFor<PlayingPageViewModel>
{
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (PlayingPageViewModel)value!;
    }

    public PlayingPageViewModel? ViewModel { get; set; }
}

public partial class PlayingPage
{
    public PlayingPage()
    {
        DataContext = App.Current.Services.GetService<PlayingPageViewModel>();
        InitializeComponent();
    }
}

public class VolumeToStringConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var percent = (int)((double)value! * 100);
        var result = percent.ToString();
        return result;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return Binding.DoNothing;
    }
}

public class CoverSizeConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length != 2) return 0;

        var width = (double)values[0];
        var height = (double)values[1];
        return Math.Min(500, Math.Min(width, height) * 0.6);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        return [];
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
        return Binding.DoNothing;
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
        return Binding.DoNothing;
    }
}