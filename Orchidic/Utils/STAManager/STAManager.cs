namespace Orchidic.Utils.STAManager;

public sealed class STAManager : ISTAManager
{
    private readonly Thread _staThread;
    private Dispatcher? _dispatcher;
    private readonly ManualResetEventSlim _ready = new();

    private bool _isDisposed;

    public STAManager()
    {
        // 创建单独的 STA 线程
        _staThread = new Thread(() =>
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            _ready.Set();
            Dispatcher.Run();
        })
        {
            IsBackground = true,
            Name = "STAManagerThread"
        };
        _staThread.SetApartmentState(ApartmentState.STA);
        _staThread.Start();

        _ready.Wait();
    }

    private Dispatcher Dispatcher
    {
        get
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(STAManager));
            return _dispatcher!;
        }
    }

    public Task RunAsync(Action action)
    {
        var tcs = new TaskCompletionSource<object?>();
        Dispatcher.BeginInvoke(new Action(() =>
        {
            try
            {
                action();
                tcs.SetResult(null);
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }
        }));
        return tcs.Task;
    }

    public Task<T> RunAsync<T>(Func<T> func)
    {
        var tcs = new TaskCompletionSource<T>();
        Dispatcher.BeginInvoke(new Action(() =>
        {
            try
            {
                var result = func();
                tcs.SetResult(result);
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }
        }));
        return tcs.Task;
    }

    public void Run(Action action)
    {
        var done = new ManualResetEventSlim();
        Exception? ex = null;

        Dispatcher.BeginInvoke(new Action(() =>
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                ex = e;
            }
            finally
            {
                done.Set();
            }
        }));

        done.Wait();
        if (ex != null) throw ex;
    }

    public T Run<T>(Func<T> func)
    {
        var done = new ManualResetEventSlim();
        T? result = default;
        Exception? ex = null;

        Dispatcher.BeginInvoke(new Action(() =>
        {
            try
            {
                result = func();
            }
            catch (Exception e)
            {
                ex = e;
            }
            finally
            {
                done.Set();
            }
        }));

        done.Wait();
        if (ex != null) throw ex;
        return result!;
    }

    public void Dispose()
    {
        if (_isDisposed) return;

        _isDisposed = true;
        Dispatcher.InvokeShutdown();
        _staThread.Join();
        _ready.Dispose();
    }
}