using Orchidic.Services.Interfaces;

namespace Orchidic.Services;

public class GlobalService : ReactiveObject, IGlobalService
{
    private bool _isZenMode;

    public bool IsZenMode
    {
        get => _isZenMode;
        set => this.RaiseAndSetIfChanged(ref _isZenMode, value);
    }
}