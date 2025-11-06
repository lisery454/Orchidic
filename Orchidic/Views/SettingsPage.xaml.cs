using Orchidic.ViewModels;

namespace Orchidic.Views;

public partial class SettingsPage : IViewFor<SettingsPageViewModel>
{
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (SettingsPageViewModel)value!;
    }

    public SettingsPageViewModel? ViewModel { get; set; }
}

public partial class SettingsPage
{
    public SettingsPage()
    {
        InitializeComponent();
    }
}