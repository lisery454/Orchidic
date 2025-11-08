using Orchidic.ViewModels;

namespace Orchidic.Views;

public partial class StatisticsPage : IViewFor<StatisticsPageViewModel>
{
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (StatisticsPageViewModel)value!;
    }

    public StatisticsPageViewModel? ViewModel { get; set; }
}

public partial class StatisticsPage
{
    public StatisticsPage()
    {
        DataContext = App.Current.Services.GetService<StatisticsPageViewModel>();
        InitializeComponent();
    }
}