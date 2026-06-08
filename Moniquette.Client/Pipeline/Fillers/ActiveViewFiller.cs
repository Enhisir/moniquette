using System.Text;
using Microsoft.Extensions.Logging;
using Moniquette.Client.Services;
using Moniquette.Client.Services.Linux;
using Moniquette.Common.Models;
using Moniquette.Common.Utils;

namespace Moniquette.Client.Pipeline.Fillers;

public class ActiveViewFiller(
    OperatingSystemService operatingSystemService,
    X11ActiveViewService x11ActiveViewService,
    WaylandGnomeActiveViewService waylandGnomeService,
    ILogger<ActiveViewFiller> logger
    ) : IReportFiller
{
    public async Task<Report> Fill(Report report, CancellationToken cancellationToken)
    {
        logger.LogDebug("Filling active window titles for process list.");

        if (!LinuxUtils.IsRoot())
        {
            // throw new WrongPrivilegesException("Linux executable must be run with sudo.");
        }
        var views = await GetViews();
        report.Processes.ForEach(p =>
        {
            var pidView = views.FirstOrDefault(v => v.Pid.Equals(p.Pid)); // TODO: проследи
            var classView = views.FirstOrDefault(v => v.Class.Equals(p.Name, StringComparison.InvariantCultureIgnoreCase));
            p.Title ??= pidView?.Title ?? classView?.Title;
        });
        return report;
    }
    
    private async Task<List<ActiveView>> GetViews()
    {
        if (OperatingSystem.IsWindows())
        {
            return GetWindowsViews();
        }

        return operatingSystemService.GetOperatingSystem() switch
        {
            Literals.Linux => await GetLinuxViews(),
            _ => []
        };
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
            return x11ActiveViewService.GetX11Views();
        }
        
        Console.WriteLine("uses wayland");
        return LinuxUtils.GetDesktopEnvironment() switch
        {
            Literals.GNOME => await waylandGnomeService.ListActiveViews(),
            Literals.KDE => [],
            _ => []
        };
    }
}
