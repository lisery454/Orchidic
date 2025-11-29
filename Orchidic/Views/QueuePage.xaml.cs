using Orchidic.Models;
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

    private void AudioListBox_PreviewDragOver(object sender, DragEventArgs e)
    {
        e.Effects = DragDropEffects.Copy;
        e.Handled = true;
    }

    private static readonly string[] AudioExtensions =
    [
        ".mp3", ".wav", ".flac", ".aac", ".m4a", ".ogg"
    ];

    private void AudioListBox_Drop(object sender, DragEventArgs e)
    {
        if (!e.Data.GetDataPresent(DataFormats.FileDrop))
            return;

        var filePaths = (string[]?)e.Data.GetData(DataFormats.FileDrop) ?? [];

        var audioFiles = filePaths
            .Where(f => AudioExtensions.Contains(Path.GetExtension(f).ToLower()))
            .Select(x => new AudioFile(x))
            .ToList();
        if (audioFiles.Count == 0)
            return;

        // 获取 DataContext (ViewModel)
        if (DataContext is QueuePageViewModel vm)
        {
            vm.AddFilesCommand.Execute(audioFiles);
        }
    }

    private void QueuePage_OnLoaded(object sender, RoutedEventArgs e)
    {
        // Dispatcher.BeginInvoke(async () =>
        // {
        //     await Task.Delay(150); // 等待 150ms
        //     if (DataContext is QueuePageViewModel vm)
        //     {
        //         if (vm.LocateCommand.CanExecute(AudioListBox))
        //         {
        //             vm.LocateCommand.Execute(AudioListBox);
        //         }
        //     }
        // }, DispatcherPriority.Loaded);
    }
}