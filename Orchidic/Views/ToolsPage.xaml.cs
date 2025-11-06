using Orchidic.ViewModels;

namespace Orchidic.Views;

public partial class ToolsPage : IViewFor<ToolsPageViewModel>
{
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (ToolsPageViewModel)value!;
    }

    public ToolsPageViewModel? ViewModel { get; set; }
}

public partial class ToolsPage
{
    public ToolsPage()
    {
        InitializeComponent();
    }
}