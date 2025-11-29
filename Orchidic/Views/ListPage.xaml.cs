using Orchidic.ViewModels;
using Orchidic.Views.Dialogs;

namespace Orchidic.Views;

public partial class ListPage : IViewFor<ListPageViewModel>
{
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (ListPageViewModel)value!;
    }

    public ListPageViewModel? ViewModel { get; set; }
}

public partial class ListPage
{
    public ListPage()
    {
        DataContext = App.Current.Services.GetService<ListPageViewModel>();
        InitializeComponent();
    }
}