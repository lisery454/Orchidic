using Orchidic.Utils;
using Orchidic.Views.Dialogs;

namespace Orchidic.ViewModels;

public class ToolsPageViewModel : ViewModelBase
{
    public ICommand SetCoverCommand { get; }

    public ToolsPageViewModel()
    {
        SetCoverCommand = ReactiveCommand.Create(() =>
        {
            var dlg = new SetCoverDialog()
            {
                Owner = Application.Current.MainWindow
            };
            dlg.ShowDialog();
        });
    }
}