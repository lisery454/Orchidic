using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive.Linq;
using Orchidic.Models;
using Orchidic.Services.Interfaces;
using Orchidic.Utils;
using ReactiveUI;

namespace Orchidic.ViewModels;

public class QueuePageViewModel : ViewModelBase
{
    private readonly IPlayerService _playerService;

    private readonly ObservableAsPropertyHelper<ObservableCollection<AudioFile>> _audioFiles;

    public ObservableCollection<AudioFile> AudioFiles => _audioFiles.Value;

    public QueuePageViewModel(IPlayerService playerService)
    {
        _playerService = playerService;
        _playerService.GetAllAudioFiles()
            .Select(list => new ObservableCollection<AudioFile>(list))
            .ToProperty(this, x => x.AudioFiles, out _audioFiles);
    }
}