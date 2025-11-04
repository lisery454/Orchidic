using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Orchidic.Utils;

public static class WindowAnimRestorer
{
    [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
    internal static extern int SetWindowLong32(HandleRef hWnd, int nIndex, int dwNewLong);

    [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
    internal static extern IntPtr SetWindowLongPtr64(HandleRef hWnd, int nIndex, IntPtr dwNewLong);

    [Flags]
    public enum WS : long
    {
        BORDER = 0x00800000L,
        CAPTION = 0x00C00000L,
        CHILD = 0x40000000L,
        CHILDWINDOW = 0x40000000L,
        CLIPCHILDREN = 0x02000000L,
        CLIPSIBLINGS = 0x04000000L,
        DISABLED = 0x08000000L,
        DLGFRAME = 0x00400000L,
        GROUP = 0x00020000L,
        HSCROLL = 0x00100000L,
        ICONIC = 0x20000000L,
        MAXIMIZE = 0x01000000L,
        MAXIMIZEBOX = 0x00010000L,
        MINIMIZE = 0x20000000L,
        MINIMIZEBOX = 0x00020000L,
        OVERLAPPED = 0x00000000L,
        OVERLAPPEDWINDOW = OVERLAPPED | CAPTION | SYSMENU | THICKFRAME | MINIMIZEBOX | MAXIMIZEBOX,
        POPUP = 0x80000000L,
        POPUPWINDOW = POPUP | BORDER | SYSMENU,
        SIZEBOX = 0x00040000L,
        SYSMENU = 0x00080000L,
        TABSTOP = 0x00010000L,
        THICKFRAME = 0x00040000L,
        TILED = 0x00000000L,
        TILEDWINDOW = OVERLAPPED | CAPTION | SYSMENU | THICKFRAME | MINIMIZEBOX | MAXIMIZEBOX,
        VISIBLE = 0x10000000L,
        VSCROLL = 0x00200000L
    }

    public static void EnableDefaultWindowAnimations(IntPtr hWnd, int nIndex = -16)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            IntPtr dwNewLong = new((long)(WS.CAPTION | WS.CLIPCHILDREN | WS.MINIMIZEBOX | WS.MAXIMIZEBOX |
                                          WS.SYSMENU | WS.SIZEBOX));
            HandleRef handle = new(null, hWnd);
            switch (IntPtr.Size)
            {
                case 8:
                    SetWindowLongPtr64(handle, nIndex, dwNewLong);
                    break;
                default:
                    SetWindowLong32(handle, nIndex, dwNewLong.ToInt32());
                    break;
            }
        }
    }

    public static void AddAnimTo(Window window)
    {
        EnableDefaultWindowAnimations(new WindowInteropHelper(window).Handle);
    }
}