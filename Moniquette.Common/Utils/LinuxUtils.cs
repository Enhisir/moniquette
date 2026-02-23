using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Moniquette.Common.Utils;

public static partial class LinuxUtils
{
    private const string XdgSessionType = "XDG_SESSION_TYPE";
    private const string XdgCurrentDesktop = "XDG_CURRENT_DESKTOP";
    private const string Wayland = "wayland";
    
    public static bool CheckSessionUsesWayland() 
        => Environment
            .GetEnvironmentVariable(XdgSessionType)
            ?.ToLower()
            .Equals(Wayland) ?? false;

    public static string? GetDesktopEnvironment()
        => Environment
            .GetEnvironmentVariable(XdgCurrentDesktop)
            ?.ToLower();
    
    public static bool IsRoot()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            return false;

        return geteuid() == 0;
    }
    
    public static Maybe<string> RunShellScript(string command)
    {
        command = command.Replace("\"", "\\\"");
        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = "bash",
                Arguments = $"-c \"{command}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var proc = Process.Start(psi);
            var output = proc?.StandardOutput.ReadToEnd().Trim() 
                         ?? throw new Exception("No process returned");
            proc.WaitForExit();
            return proc.ExitCode == 0 
                ? Maybe<string>.Success(output)
                : Maybe<string>.Failure(output);
        }
        catch (Exception ex)
        {
            return Maybe<string>.Failure($"{ex.GetType().FullName}: {ex.Message}");
        }
    }
    
    [LibraryImport("libc")]
    private static partial uint geteuid();
}