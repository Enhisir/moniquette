using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Moniquette.Common.Utils;

public delegate bool CallBack(int hWnd, int lParam);

[System.Runtime.Versioning.SupportedOSPlatform(Literals.Windows)]
public static class WindowsUtils
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