using System.Text;
using Hardware.Info;
using Microsoft.Extensions.Logging;
using Moniquette.Client.Services;
using Moniquette.Common.Models;
using Moniquette.Common.Utils;

namespace Moniquette.Client.Pipeline.Fillers;

public class ActiveViewFiller(
    IHardwareInfo hardwareInfo,
    WmctrlService wmctrlService,
    GnomeWindowsExtensionService gnomeService,
    ILogger<ActiveViewFiller> logger
    ) : IReportFiller
{
    public async Task<Report> Fill(Report report, CancellationToken cancellationToken)
    {
        if (!LinuxUtils.IsRoot())
        {
            // throw new WrongPrivilegesException("Linux executable must be run with sudo.");
        }
        report.Views = await GetViews();
        return report;
    }
    
    private async Task<List<ActiveView>> GetViews()
    {
        return GetOperatingSystemName() switch
        {
            Literals.Windows => GetWindowsViews(),
            Literals.Linux => await GetLinuxViews(),
            Literals.MacOS => [],
            _ => []
        };
    }

    private string GetOperatingSystemName()
    {
        var fullname = hardwareInfo.OperatingSystem.Name.ToLower();
        if (fullname.Contains("windows")) return Literals.Windows;
        if (fullname.Contains("linux")) return Literals.Linux;
        if (fullname.Contains("macos")) return Literals.MacOS;
        return fullname;
    }
        
    [System.Runtime.Versioning.SupportedOSPlatform(Literals.Windows)]
    private static List<ActiveView> GetWindowsViews()
    {
        var views = new List<ActiveView>();
        var callback = new CallBack((hWnd, pId) =>
        {
            var classNameBuffer = new StringBuilder(256);
            var titleBuffer = new StringBuilder(256);
            _ = WindowsUtils.GetClassName(hWnd, classNameBuffer, classNameBuffer.Capacity);
            _ = WindowsUtils.GetWindowText(hWnd, titleBuffer, titleBuffer.Capacity);
            views.Add(new ActiveView {
                Id = hWnd,
                Pid = pId,
                Class = classNameBuffer.ToString(),
                Title = titleBuffer.ToString()
            });
            return true;
        });
        WindowsUtils.EnumWindows(callback, IntPtr.Zero);
        return views;
    }

    private async Task<List<ActiveView>> GetLinuxViews()
    {
        if (!LinuxUtils.CheckSessionUsesWayland())
        {
            return wmctrlService.GetX11Views();
        }
        
        Console.WriteLine("uses wayland");
        return LinuxUtils.GetDesktopEnvironment() switch
        {
            Literals.GNOME => await gnomeService.ListActiveViews(),
            Literals.KDE => [],
            _ => []
        };
    }

    /*
    private List<string> GetMacOSViews()
    {
        var result = new List<string>();
        var outputMac = ShellScriptRunner.Run(
            "osascript",
            "-e 'tell application \"System Events\" to get name of (processes where frontmost is true)'");
        result.AddRange(outputMac.Split(',').Select(x => x.Trim()));
        return result;
    }
    */
}