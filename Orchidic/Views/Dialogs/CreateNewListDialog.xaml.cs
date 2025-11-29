using System.Runtime.CompilerServices;
using Ookii.Dialogs.Wpf;
using Orchidic.Utils;

namespace Orchidic.Views.Dialogs;

public partial class CreateNewListDialog : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}

public partial class CreateNewListDialog
{
    public (string, string)? Result { get; private set; }

    private string _listName = string.Empty;

    public string ListName
    {
        get => _listName;
        set
        {
            if (_listName == value) return;
            _listName = value;
            OnPropertyChanged();
        }
    }


    private string _dirPath = string.Empty;

    public string DirPath
    {
        get => _dirPath;
        private set
        {
            if (_dirPath == value) return;
            _dirPath = value;
            OnPropertyChanged();
        }
    }

    public CreateNewListDialog()
    {
        WindowCornerRestorer.ApplyRoundCorner(this);
        InitializeComponent();

        DataContext = this;
    }

    private void Ok_Click(object sender, RoutedEventArgs e)
    {
        Result = (ListName, DirPath);
        DialogResult = true;
        Close();
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        Result = null;
        DialogResult = false;
        Close();
    }

    private void Button_SelectFolder_OnClick(object sender, RoutedEventArgs e)
    {
        var dlg = new VistaFolderBrowserDialog
        {
            Description = "请选择文件夹",
            UseDescriptionForTitle = true,
            ShowNewFolderButton = false // 是否允许用户新建文件夹
        };

        var result = dlg.ShowDialog(this); // this = Owner Window
        if (result != true) return;

        var folder = dlg.SelectedPath;
        DirPath = folder;
    }
}