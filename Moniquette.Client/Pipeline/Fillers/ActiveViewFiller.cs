using System.Text;
using Hardware.Info;
using Moniquette.Common;
using Moniquette.Common.Helpers;
using Moniquette.Common.Models;

namespace Moniquette.Client.Pipeline.Fillers;

public class ActiveViewFiller(IHardwareInfo hardwareInfo) : IReportFiller
{
    public Task<Report> Fill(Report report, CancellationToken cancellationToken)
    {
        report.Views = GetViews();
        return Task.FromResult(report);
    }
    
    private List<string> GetViews()
    {
        return hardwareInfo.OperatingSystem.Name.ToLower() switch
        {
            TargetSystems.Windows => GetWindowsViews(),
            TargetSystems.Linux => GetLinuxViews(),
            TargetSystems.MacOS => GetMacOSViews(),
            _ => []
        };
    }

    private List<string> GetWindowsViews()
    {
        var result = new List<string>();
        var handle = WindowsHelper.GetForegroundWindow();
        var buffer = new StringBuilder(256);
        WindowsHelper.GetWindowText(handle, buffer, buffer.Capacity);
        result.Add(buffer.ToString().Trim());
        return result;
    }

    private List<string> GetLinuxViews()
    {
        var result = new List<string>();
        var outputLinux = ShellScriptRunner.Run("xdotool", "getwindowfocus getwindowname"); // не работает в Wayland. Вообще.
        if (!string.IsNullOrWhiteSpace(outputLinux)) result.Add(outputLinux.Trim());
        return result;
    }

    private List<string> GetMacOSViews()
    {
        var result = new List<string>();
        var outputMac = ShellScriptRunner.Run(
            "osascript", 
            "-e 'tell application \"System Events\" to get name of (processes where frontmost is true)'");
        result.AddRange(outputMac.Split(',').Select(x => x.Trim()));
        return result;
    }
}