namespace Orchidic.Models;

public class Statistics
{
    public Dictionary<string, int> CountMap { get; set; } = new();

    public TimeSpan TotalTime;
}