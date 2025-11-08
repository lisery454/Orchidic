using Orchidic.ViewModels;

namespace Orchidic.Views;

public partial class SearchPage : IViewFor<SearchPageViewModel>
{
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (SearchPageViewModel)value!;
    }

    public SearchPageViewModel? ViewModel { get; set; }
}

public partial class SearchPage
{
    public SearchPage()
    {
        DataContext = App.Current.Services.GetService<SearchPageViewModel>();
        InitializeComponent();
    }
}