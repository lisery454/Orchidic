using Orchidic.Models;

namespace Orchidic.Services.Interfaces;

public interface IAudioQueueService : IReactiveObject
{
    AudioQueue AudioQueue { get; } 
    BitmapSource? CurrentCover { get; }
    BitmapSource? CurrentBlurCover { get; }
}