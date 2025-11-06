using Orchidic.Utils;
using Orchidic.ViewModels.Components;

namespace Orchidic.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public SideMenuViewModel SideMenuViewModel { get; } = new();
}