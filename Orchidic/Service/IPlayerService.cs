using System;

namespace Orchidic.Service;

public interface IPlayerService
{
    void LoadFile(string path);
    TimeSpan GetTotalTime();
    TimeSpan GetCurrentTime();
    void Play();
    void Pause();
    void Seek(TimeSpan targetTime);
    bool IsPlaying();
}