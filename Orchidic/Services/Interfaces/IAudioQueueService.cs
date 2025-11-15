using Orchidic.Models;

namespace Orchidic.Services.Interfaces;

public interface IAudioQueueService : IReactiveObject
{
    AudioFile? CurrentAudioFile { get; }
    int CurrentIndex { get; set; }
    ObservableCollection<AudioFile> AudioFiles { get; }
    BitmapSource CurrentCover { get; }
    string Title { get; }
    BitmapSource? CurrentBlurCover { get; }
}