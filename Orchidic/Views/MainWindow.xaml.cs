using Orchidic.Utils;
using Orchidic.ViewModels;

namespace Orchidic.Views;

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
