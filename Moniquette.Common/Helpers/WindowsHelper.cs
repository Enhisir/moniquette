using System.Runtime.InteropServices;
using System.Text;

namespace Moniquette.Common.Helpers;

public delegate bool CallBack(int hWnd, int lParam);

[System.Runtime.Versioning.SupportedOSPlatform(Literals.Windows)]
public static class WindowsHelper
{
    [DllImport("user32.dll")]
    public static extern int GetForegroundWindow();

    [DllImport("user32.dll")]
    public static extern bool EnumWindows(CallBack lpEnumFunc, IntPtr lParam);
    
    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern int GetWindowText(int hWnd, StringBuilder text, int count);

    [DllImport("user32.dll")]
    public static extern bool IsWindowVisible(int hWnd);
}