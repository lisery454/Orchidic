using Orchidic.Utils;

namespace Orchidic.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private string _greeting = "Lisery";

    public string Greeting
    {
        get => _greeting;
        set => this.RaiseAndSetIfChanged(ref _greeting, value);
    }
}