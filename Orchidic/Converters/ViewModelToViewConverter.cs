using Orchidic.Utils;
using Orchidic.ViewModels;
using Orchidic.Views;

namespace Orchidic.Converters;

public class ViewModelToViewConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var vm = value as ViewModelBase;

        return vm switch
        {
            MainWindowViewModel => App.Current.Services.GetService<MainWindow>(),
            PlayingPageViewModel => App.Current.Services.GetService<PlayingPage>(),
            QueuePageViewModel => App.Current.Services.GetService<QueuePage>(),
            ListPageViewModel => App.Current.Services.GetService<ListPage>(),
            StatisticsPageViewModel => App.Current.Services.GetService<StatisticsPage>(),
            ToolsPageViewModel => App.Current.Services.GetService<ToolsPage>(),
            SettingsPageViewModel => App.Current.Services.GetService<SettingsPage>(),
            _ => null
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return Binding.DoNothing;
    }
}