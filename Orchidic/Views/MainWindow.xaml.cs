using Orchidic.Utils;
using Orchidic.ViewModels;

namespace Orchidic.Views;

public partial class MainWindow
{
    public MainWindow()
    {
        Loaded += Window_Loaded;
        Activated += Window_Activated;
        WindowCornerRestorer.ApplyRoundCorner(this);
        DataContext = App.Current.Services.GetService<MainWindowViewModel>();
        InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        WindowAnimRestorer.AddAnimTo(this);
    }
    
    private void ForceRefresh()
    {
        Dispatcher.BeginInvoke(new Action(() =>
        {
            var hwndSource = PresentationSource.FromVisual(this) as HwndSource;
            if (hwndSource?.CompositionTarget != null)
            {
                // 强制渲染目标更新
                hwndSource.CompositionTarget.RootVisual = null;
                hwndSource.CompositionTarget.RootVisual = this;
            }
        }), DispatcherPriority.Render);
    }

    private void Window_Activated(object? sender, EventArgs e)
    {
        ForceRefresh();
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