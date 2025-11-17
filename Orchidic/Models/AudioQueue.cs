namespace Orchidic.Models;

public class AudioQueue : ReactiveObject
{
    public AudioQueue()
    {
        _currentIndex = 0;
        AudioFiles = [];


        const string musicFolder = @"D:\Music";
        var files = Directory.GetFiles(musicFolder, "*.mp3", SearchOption.AllDirectories);
        foreach (var file in files)
        {
            AudioFiles.Add(new AudioFile(file));
        }
    }

    public ObservableCollection<AudioFile> AudioFiles { get; }


    private int _currentIndex;

    public int CurrentIndex
    {
        get => _currentIndex;
        set
        {
            int newValue;
            if (value >= AudioFiles.Count)
                newValue = 0;
            else if (value < 0)
                newValue = AudioFiles.Count - 1;
            else
                newValue = value;

            this.RaiseAndSetIfChanged(ref _currentIndex, newValue);
            this.RaisePropertyChanged(nameof(CurrentAudioFile));
        }
    }

    public AudioFile? CurrentAudioFile
    {
        get
        {
            if (CurrentIndex >= AudioFiles.Count || CurrentIndex < 0) return null;
            return AudioFiles[CurrentIndex];
        }
    }

    public void TrySetCurrentAudioFile(AudioFile audioFile)
    {
        var index = AudioFiles.IndexOf(audioFile);

        if (index >= 0)
        {
            CurrentIndex = index;
        }
    }
}