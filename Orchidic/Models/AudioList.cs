namespace Orchidic.Models;

public class AudioList : ReactiveObject
{
    public ObservableCollection<string> AudioFilePaths { get; } = [];

    private string _name = string.Empty;

    public string Name
    {
        get => _name;
        set => this.RaiseAndSetIfChanged(ref _name, value);
    }

    public AudioList(string name, string[] audioFilePaths)
    {
        Name = name;
        AudioFilePaths = new ObservableCollection<string>(audioFilePaths);
    }
}