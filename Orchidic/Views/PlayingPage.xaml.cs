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