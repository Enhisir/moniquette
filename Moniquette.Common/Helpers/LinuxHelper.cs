using System.Runtime.InteropServices;

namespace Moniquette.Common.Helpers;

public static partial class LinuxHelper
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
    
    [LibraryImport("libc")]
    private static partial uint geteuid();
}