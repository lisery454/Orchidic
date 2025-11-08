using Orchidic.Utils;
using Orchidic.ViewModels;

namespace Orchidic.Views;

public partial class MainWindow : IViewFor<MainWindowViewModel>
{
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (MainWindowViewModel)value!;
    }

    public MainWindowViewModel? ViewModel { get; set; }
}

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

    private void ToggleMaximize()
    {
        WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
    }

    private void MinButton_OnClick(object? sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void MaxButton_OnClick(object? sender, RoutedEventArgs e)
    {
        ToggleMaximize();
    }

    private void CloseButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
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