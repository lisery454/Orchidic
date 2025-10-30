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
            new AudioFile(@"D:\Music\[DJMAX RESPECT] Groovin Up - Mycin.T.mp3"),
            new AudioFile(@"D:\Music\[Lanota-Arcaea] Dream goes on - Tiny Minim .mp3"),
            new AudioFile(@"D:\Music\[Might Fall In Love] LOVE PSYCHEDELICO.mp3"),
            new AudioFile(@"D:\Music\[Muse Dash] 恋爱回避依存症 - Matthiola Records.mp3"),
            new AudioFile(@"D:\Music\[ON YOUR GLORY DAY] わっふゑ - I'M ALIVE.mp3"),
            new AudioFile(@"D:\Music\「 Nightcore 」 → Kimi ni Tsutaetakute.mp3"),
            new AudioFile(@"D:\Music\【Arcaea】ユアミトス『san skia』.mp3"),
            new AudioFile(@"D:\Music\【BOFU2016】Chronomia  Lime.mp3"),
            new AudioFile(@"D:\Music\【G2R2018】 R.I.P..mp3"),
            new AudioFile(@"D:\Music\【HD】 六兆年と一夜物語 【IAオリジナル曲・PV付】- KEMU VOXX.mp3"),
            new AudioFile(@"D:\Music\Aiobahn  宙でおやすみ feat 長瀬有花 .mp3"),
            new AudioFile(@"D:\Music\Aiobahn +81 feat. ななひら & P丸様。- 天天天国地獄国 (Official Music Video).mp3"),
            new AudioFile(@"D:\Music\Aiobahn feat KOTOKO  INTERNET YAMERO .mp3"),
            new AudioFile(@"D:\Music\Akira Complex - Ether Strike.mp3"),
            new AudioFile(@"D:\Music\anoスマイルあげない.mp3"),
            new AudioFile(@"D:\Music\AOTQ 長瀬有花  ホライゾン.mp3"),
            new AudioFile(@"D:\Music\conflict  siromaru  cranky.mp3"),
            new AudioFile(@"D:\Music\DECO27  ゴーストルール feat 初音ミク.mp3"),
            new AudioFile(@"D:\Music\eufonius - メグメル (Mag mell) (Cuckool mix 2007).mp3"),
            new AudioFile(@"D:\Music\inside me.mp3"),
            new AudioFile(@"D:\Music\Muse Dash OST  Lights of Muse .mp3"),
            new AudioFile(@"D:\Music\Neru  ロストワンの号哭Lost Ones Weeping feat Kagamine Rin.mp3"),
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