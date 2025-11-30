namespace Orchidic.Models;

public class Statistics : INotifyPropertyChanged
{
    public Dictionary<string, int> CountMap { get; set; } = new();

    public TimeSpan TotalTime;

    [YamlIgnore] // 如果你用 YAML，避免重复序列化
    public TimeSpan TotalTimeBindable
    {
        get => TotalTime;
        set
        {
            if (TotalTime != value)
            {
                TotalTime = value;
                OnPropertyChanged(nameof(TotalTimeBindable));
            }
        }
    }

    // 可选：CountMap 有变化时你手动 Raise
    public void NotifyCountMapChanged()
    {
        OnPropertyChanged(nameof(CountMap));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(string name)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}