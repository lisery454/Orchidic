using System.Collections.Generic;

namespace Orchidic.Models;

public class AudioQueue
{
    public AudioQueue()
    {
        CurrentIndex = 0;
        AudioFiles =
        [
            new AudioFile(@"D:\Music\【originalMV】自分後回し⧸P丸様。.mp3"),
            new AudioFile(@"D:\Music\【とあ】拼凑的断音【初音ミク】.mp3"),
            new AudioFile(@"D:\Music\#G2R2018  Destr0yer (feat. Nikki Simmons).mp3")
        ];
    }

    public List<AudioFile> AudioFiles { get; }


    private int _currentIndex;

    public int CurrentIndex
    {
        get => _currentIndex;
        set
        {
            if (_currentIndex == value) return;
            if (value >= AudioFiles.Count)
            {
                _currentIndex = 0;
            }
            else if (value < 0)
            {
                _currentIndex = AudioFiles.Count - 1;
            }
            else
            {
                _currentIndex = value;
            }
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
}