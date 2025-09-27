using System.Runtime.InteropServices;
using System.Text;

namespace Moniquette.Common.Helpers;

[System.Runtime.Versioning.SupportedOSPlatform(TargetSystems.Windows)]
public static class WindowsHelper
{
    [DllImport("user32.dll")]
    public static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    public static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);
}