using System.Diagnostics;

namespace Moniquette.Common;

public static class ShellScriptRunner
{
    public static string Run(string file, string args)
    {
        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = file,
                Arguments = args,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var proc = Process.Start(psi);
            return proc?.StandardOutput.ReadToEnd().Trim() ?? string.Empty;
        }
        catch (Exception ex)
        {
            return $"ERROR: {ex.Message}";
        }
    }
}