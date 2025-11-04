using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace Orchidic.Utils;

internal static class ProcessManager
{
    private static Mutex? _processLock;
    private static bool _hasLock;

    public static void GetProcessLock()
    {
        // 全局锁，锁名称可以自定义。
        _processLock = new Mutex(false, $"Global\\Lisery.ECGPlatform.Toolkit[{GetUid()}]", out _hasLock);
        if (!_hasLock)
        {
            ActiveWindow();
            Environment.Exit(0);
        }
    }

    private static string GetUid()
    {
        var bytes = Encoding.UTF8.GetBytes(Assembly.GetExecutingAssembly().Location);
        using (var md5 = MD5.Create())
        {
            bytes = md5.ComputeHash(bytes);
        }

        return BitConverter.ToString(bytes);
    }

    public static void ActiveWindow()
    {
        using var p = Process.GetCurrentProcess();
        var pName = p.ProcessName;
        var temp = Process.GetProcessesByName(pName);
        foreach (var item in temp)
        {
            if (item.MainModule!.FileName != p.MainModule!.FileName) continue;
            var handle = item.MainWindowHandle;
            SwitchToThisWindow(handle, true);
            break;
        }
    }

    public static void ReleaseLock()
    {
        if (_processLock != null && _hasLock)
        {
            _processLock.Dispose();
            _hasLock = false;
        }
    }

    [DllImport("user32.dll")]
    public static extern void SwitchToThisWindow(IntPtr hWnd, bool fAltTab);
}