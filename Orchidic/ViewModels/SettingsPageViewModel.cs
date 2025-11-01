using System.Windows.Input;
using Avalonia;
using Avalonia.Styling;
using Orchidic.Utils;
using ReactiveUI;

namespace Orchidic.ViewModels;

public class SettingsPageViewModel : ViewModelBase
{
    public ICommand ToggleThemeCommand { get; }

    public SettingsPageViewModel()
    {
        ToggleThemeCommand = ReactiveCommand.Create(() =>
        {
            var app = (App)Application.Current!;
            app.RequestedThemeVariant = app.RequestedThemeVariant == ThemeVariant.Light
                ? ThemeVariant.Dark
                : ThemeVariant.Light;
        });
    }
}