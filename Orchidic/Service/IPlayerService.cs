using System;
using Orchidic.Models;

namespace Orchidic.Service;

public interface IPlayerService
{
    AudioFile? GetCurrentAudioFile();
    void LoadFile(AudioFile? file);
    void Next();
    void Prev();
    TimeSpan GetTotalTime();
    TimeSpan GetCurrentTime();
    void Play();
    void Pause();
    void Seek(TimeSpan targetTime);
    bool IsPlaying();
    float GetVolume();
    void SetVolume(float volume);
}