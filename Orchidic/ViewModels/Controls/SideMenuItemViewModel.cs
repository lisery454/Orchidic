using System.Windows.Input;
using Orchidic.Models;
using ReactiveUI;

namespace Orchidic.ViewModels;

public class SideMenuItemViewModel(string text, PageType type, ICommand selectItemCommand) : ReactiveObject
{
    private bool _isSelected;

    public string Text { get; } = text;
    public PageType Type { get; } = type;
    public ICommand SelectItemCommand { get; } = selectItemCommand;

    public bool IsSelected
    {
        get => _isSelected;
        set => this.RaiseAndSetIfChanged(ref _isSelected, value);
    }
}