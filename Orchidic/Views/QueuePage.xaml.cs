using Orchidic.ViewModels;

namespace Orchidic.Views;

public partial class QueuePage : IViewFor<QueuePageViewModel>
{
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (QueuePageViewModel)value!;
    }

    public QueuePageViewModel? ViewModel { get; set; }
}

public partial class QueuePage
{
    public QueuePage()
    {
        DataContext = App.Current.Services.GetService<QueuePageViewModel>();
        InitializeComponent();
    }
}