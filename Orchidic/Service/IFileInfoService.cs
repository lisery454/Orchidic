using Avalonia.Media.Imaging;

namespace Orchidic.Service;

public interface IFileInfoService
{
    Bitmap GetCoverFromAudio(string path);
    
    Bitmap GetDefaultCover();
    
    string GetTitleFromAudio(string path);
    
    string GetDefaultTitle();
}