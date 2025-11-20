namespace Orchidic.Models;

public class AudioQueue : ReactiveObject
{
    public AudioQueue(List<AudioFile> audioFiles)
    {
        _currentIndex = 0;
        AudioFiles = [];
        AudioFiles = new ObservableCollection<AudioFile>(audioFiles);
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

    public void Remove(AudioFile audioFile)
    {
        var index = AudioFiles.IndexOf(audioFile);

        if (index >= 0)
        {
            if (CurrentIndex == index)
            {
                AudioFiles.RemoveAt(index);
                CurrentIndex = index; // 下一首
            }
            else if (CurrentIndex > index)
            {
                AudioFiles.RemoveAt(index);
                CurrentIndex -= 1; // 上移一位，还是同一首
            }
            else
            {
                AudioFiles.RemoveAt(index); // 同一首
            }
        }
    }
}