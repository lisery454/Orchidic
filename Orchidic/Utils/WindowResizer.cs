namespace Orchidic.Utils;
// ReSharper disable PossibleNullReferenceException
#pragma warning disable CS8605 // Unboxing a possibly null value.

public class WindowResizer
{
    /// <summary>
    /// use in Window.SourceInitialized()
    /// </summary>
    /// <param name="window"></param>
    public static void CoverResizeIssue(Window window)
    {
        var mWindowHandle = new WindowInteropHelper(window).Handle;
        HwndSource.FromHwnd(mWindowHandle).AddHook(WindowProc);
    }

    private static IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        switch (msg)
        {
            case 0x0024:
                WmGetMinMaxInfo(hwnd, lParam);
                break;
        }

        return IntPtr.Zero;
    }


    private static void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam)
    {
        POINT lMousePosition;
        GetCursorPos(out lMousePosition);

        IntPtr lCurrentScreen = MonitorFromPoint(lMousePosition, MonitorOptions.MONITOR_DEFAULTTONEAREST);

        MINMAXINFO lMmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));

        MONITORINFO lCurrentScreenInfo = new MONITORINFO();

        if (GetMonitorInfo(lCurrentScreen, lCurrentScreenInfo) == false)
        {
            return;
        }

        //Position relative pour notre fenêtre
        lMmi.ptMaxPosition.X = lCurrentScreenInfo.rcWork.Left - lCurrentScreenInfo.rcMonitor.Left;
        lMmi.ptMaxPosition.Y = lCurrentScreenInfo.rcWork.Top - lCurrentScreenInfo.rcMonitor.Top;
        lMmi.ptMaxSize.X = lCurrentScreenInfo.rcWork.Right - lCurrentScreenInfo.rcWork.Left;
        lMmi.ptMaxSize.Y = lCurrentScreenInfo.rcWork.Bottom - lCurrentScreenInfo.rcWork.Top;

        Marshal.StructureToPtr(lMmi, lParam, true);
    }


    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool GetCursorPos(out POINT lpPoint);


    [DllImport("user32.dll", SetLastError = true)]
    static extern IntPtr MonitorFromPoint(POINT pt, MonitorOptions dwFlags);

    enum MonitorOptions : uint
    {
        MONITOR_DEFAULTTONULL = 0x00000000,
        MONITOR_DEFAULTTOPRIMARY = 0x00000001,
        MONITOR_DEFAULTTONEAREST = 0x00000002
    }


    [DllImport("user32.dll")]
    static extern bool GetMonitorInfo(IntPtr hMonitor, MONITORINFO lpmi);


    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;

        public POINT(int x, int y)
        {
            X = x;
            Y = y;
        }
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct MINMAXINFO
    {
        public POINT ptReserved;
        public POINT ptMaxSize;
        public POINT ptMaxPosition;
        public POINT ptMinTrackSize;
        public POINT ptMaxTrackSize;
    };


    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public class MONITORINFO
    {
        public int cbSize = Marshal.SizeOf(typeof(MONITORINFO));
        public RECT rcMonitor = new RECT();
        public RECT rcWork = new RECT();
        public int dwFlags = 0;
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left, Top, Right, Bottom;

        public RECT(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }
    }
}