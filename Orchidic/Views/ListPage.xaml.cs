using Orchidic.ViewModels;

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
        InitializeComponent();
    }
}