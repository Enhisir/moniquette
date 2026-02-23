using System.Diagnostics;
using Moniquette.ProcessObserver.Models;

namespace Moniquette.ProcessObserver.ProcessPipeline;

public class LinuxProcessHandler : IProcessHandler
{
    private IEnumerable<ProcessInfo> GetLinuxApps(List<int>? previousPids = null)
    {
        var executablePathAlreadyExists = new HashSet<string>(); // чтобы не повторялись пути

        var processInfos = Process
            .GetProcesses()
            .Where(p => !(previousPids?.Contains(p.Id) ?? false))
            .Where(p => !(previousPids?.Contains(p.SessionId) ?? false))
            .Where(p => p.SessionId != 0 && p.Responding)
            .Select(p => new ProcessInfo
            {
                Pid = p.Id,
                Name = p.ProcessName,
                ExecutablePath = p.MainModule?.FileName.Trim() ?? string.Empty
            })
            .Where(p => !p.ExecutablePath.Equals(string.Empty))
            .Where(pInfo => executablePathAlreadyExists.Add(pInfo.ExecutablePath))
            .ToList();
        return processInfos;
    }

    public IEnumerable<ProcessInfo> Invoke(IEnumerable<ProcessInfo>? processInfos = null)
    {
        var previousPids = processInfos?.ToList() ?? [];
        var linuxApps = GetLinuxApps(
            previousPids
                .Select(p => p.Pid)
                .ToList());
        return previousPids.Concat(linuxApps);
    }
}