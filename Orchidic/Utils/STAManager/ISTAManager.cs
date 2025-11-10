namespace Orchidic.Utils.STAManager;

public interface ISTAManager : IDisposable
{
    /// <summary>
    /// 在 STA 线程中执行任务（异步）
    /// </summary>
    Task RunAsync(Action action);

    /// <summary>
    /// 在 STA 线程中执行任务并返回结果（异步）
    /// </summary>
    Task<T> RunAsync<T>(Func<T> func);

    /// <summary>
    /// 在 STA 线程中执行任务（同步阻塞）
    /// </summary>
    void Run(Action action);

    /// <summary>
    /// 在 STA 线程中执行任务并返回结果（同步阻塞）
    /// </summary>
    T Run<T>(Func<T> func);
}