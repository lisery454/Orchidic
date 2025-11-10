namespace Orchidic.Services.Interfaces;

public interface IFileInfoService
{
    BitmapSource GetCoverFromAudio(string path);

    BitmapSource GetDefaultCover();

    string GetTitleFromAudio(string path);

    string GetDefaultTitle();

    Task<BitmapSource> GetBlurCoverFromCover(BitmapSource cover, string? audioPath);
}