using System.Collections.Concurrent;
using System.Reactive.Concurrency;
using System.Reactive.Subjects;
using Orchidic.Models;
using Orchidic.Services.Interfaces;
using Orchidic.Utils.SettingManager;

namespace Orchidic.Services;

public class PlayerService : ReactiveObject, IPlayerService
{
    #region fields

    private readonly BlockingCollection<Action> _taskQueue = new();
    private readonly Thread _playerThread;

    private WaveOutEvent? _outputDevice;
    private AudioFileReader? _audioFileReader;

    private readonly Subject<float> _seekSubject = new();

    #endregion

    #region properties

    private bool _isPlaying;
    private AudioFile? _currentAudioFile;

    public bool IsPlaying
    {
        get => _isPlaying;
        private set => this.RaiseAndSetIfChanged(ref _isPlaying, value);
    }

    public AudioFile? CurrentAudioFile
    {
        get => _currentAudioFile;
        private set => this.RaiseAndSetIfChanged(ref _currentAudioFile, value);
    }

    public TimeSpan CurrentTime => _audioFileReader?.CurrentTime ?? TimeSpan.Zero;
    public TimeSpan TotalTime => _audioFileReader?.TotalTime ?? TimeSpan.Zero;

    private float _progress;

    public float Progress
    {
        get => _progress;
        set
        {
            if (Math.Abs(_progress - value) < 0.001f) return;
            this.RaiseAndSetIfChanged(ref _progress, value);
            Seek(value);
        }
    }

    public IObservable<TimeSpan> CurrentTimeObservable =>
        Observable.Interval(TimeSpan.FromMilliseconds(200))
            .Select(_ => _audioFileReader?.CurrentTime ?? TimeSpan.Zero)
            .DistinctUntilChanged();

    private readonly BehaviorSubject<TimeSpan> _totalTimeSubject = new(TimeSpan.Zero);
    public IObservable<TimeSpan> TotalTimeObservable => _totalTimeSubject.AsObservable();

    public IObservable<float> ProgressObservable =>
        CurrentTimeObservable
            .CombineLatest(TotalTimeObservable,
                (current, total) => total.TotalSeconds == 0 ? 0 : (float)(current.TotalSeconds / total.TotalSeconds));

    private float _volume;

    public float Volume
    {
        get => _volume;
        set => this.RaiseAndSetIfChanged(ref _volume, value);
    }

    public IObservable<float> VolumeObservable => this.WhenAnyValue(x => x.Volume);

    #endregion

    #region constructors

    public PlayerService(ISettingManager settingManager)
    {
        _volume = settingManager.CurrentSetting.Volume;

        VolumeObservable.Subscribe(vol =>
        {
            // 更新底层 NAudio 音量
            if (_audioFileReader != null)
                _audioFileReader.Volume = vol;

            // 保存设置
            settingManager.CurrentSetting.Volume = vol;
        });

        StartProgressTimer();


        // 启动音频专用线程
        _playerThread = new Thread(RunLoop)
        {
            IsBackground = true,
            Name = "AudioPlayerThread"
        };
        _playerThread.Start();
    }

    #endregion

    #region events

    public event EventHandler? PlaybackEnded;

    #endregion

    #region public methods

    public Task PlayAsync(AudioFile audioFile)
    {
        var tcs = new TaskCompletionSource();
        EnqueueTask(() =>
        {
            try
            {
                StopInternal();
                _audioFileReader = new AudioFileReader(audioFile.FilePath) { Volume = _volume };
                _outputDevice = new WaveOutEvent();
                _outputDevice.Init(_audioFileReader);
                _outputDevice.PlaybackStopped += (_, _) =>
                {
                    IsPlaying = false;
                    PlaybackEnded?.Invoke(this, EventArgs.Empty);
                };
                _outputDevice.Play();
                IsPlaying = true;
                CurrentAudioFile = audioFile;
                _totalTimeSubject.OnNext(_audioFileReader.TotalTime);
                tcs.SetResult();
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }
        });
        return tcs.Task;
    }

    public void Pause() => EnqueueTask(() =>
    {
        _outputDevice?.Pause();
        IsPlaying = false;
    });

    public void Resume() => EnqueueTask(() =>
    {
        _outputDevice?.Play();
        IsPlaying = true;
    });

    public void Stop() => EnqueueTask(StopInternal);

    public void Seek(float progress)
    {
        var reader = _audioFileReader; // 本地副本

        if (reader != null)
        {
            var seekTime = TotalTime * progress;

            if (Math.Abs((seekTime - CurrentTime).TotalMilliseconds) > 1000)
            {
                reader.CurrentTime = seekTime;
            }
        }
    }

    public void Dispose()
    {
        _taskQueue.CompleteAdding();
        _playerThread.Join();
        StopInternal();
    }

    #endregion

    #region private methods

    private void StartProgressTimer()
    {
        var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(200) };
        timer.Tick += (_, _) => UpdateProgressFromAudio();
        timer.Start();
    }

    private void UpdateProgressFromAudio()
    {
        if (_currentAudioFile != null)
        {
            Progress = (float)(CurrentTime.TotalSeconds / TotalTime.TotalSeconds);
        }
    }


    private void RunLoop()
    {
        foreach (var action in _taskQueue.GetConsumingEnumerable())
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[AudioThread Error] {ex}");
            }
        }
    }

    private void EnqueueTask(Action action)
    {
        if (!_taskQueue.IsAddingCompleted)
            _taskQueue.Add(action);
    }

    private void StopInternal()
    {
        _outputDevice?.Stop();
        _outputDevice?.Dispose();
        _outputDevice = null;
        _audioFileReader?.Dispose();
        _audioFileReader = null;
        CurrentAudioFile = null;
        IsPlaying = false;
    }

    #endregion
}