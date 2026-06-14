using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Moniquette.Common.Utils;

public delegate bool WindowEnumCallback(IntPtr hWnd, IntPtr lParam);

[System.Runtime.Versioning.SupportedOSPlatform(Literals.Windows)]
public static class WindowsUtils
{
    [DllImport("user32.dll")]
    public static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    public static extern bool EnumWindows(WindowEnumCallback lpEnumFunc, IntPtr lParam);
    
    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern int GetWindowTextLength(IntPtr hWnd);

    [DllImport("user32.dll")]
    public static extern bool IsWindowVisible(IntPtr hWnd);

    [DllImport("user32.dll")]
    public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);
    
    public static string RunShellScript(string command)
    {
        command = command.Replace("\"", "\\\"");
        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"-ExecutionPolicy Bypass -Command \"{command}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var proc = Process.Start(psi);
            return proc?.StandardOutput.ReadToEnd().Trim() ?? string.Empty;
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.Message}";
        }
    }
}
