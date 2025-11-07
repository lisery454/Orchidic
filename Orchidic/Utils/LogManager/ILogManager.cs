namespace Orchidic.Utils.LogManager;

public interface ILogManager
{
    void Info(string message);
    void Warning(string message);
    void Error(string message);
}