using Orchidic.Models;

namespace Orchidic.Services.Interfaces;

public interface IAudioQueueService : IReactiveObject
{
    AudioFile? CurrentAudioFile { get; }
    int CurrentIndex { get; set; }
    IObservable<IReadOnlyCollection<AudioFile>> AudioFilesObservable { get; }
    BitmapSource CurrentCover { get; }
    string Title { get; }
    BitmapSource? CurrentBlurCover { get; }
}