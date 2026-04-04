using Moniquette.Common.Exceptions;
using Moniquette.Common.Models;
using Moniquette.Common.Utils;

namespace Moniquette.ProcessObserver.ProcessPipeline;

public class FlatpakProcessHandler : IProcessHandler
{
    private const string GetFlatpakRunningApps = "flatpak ps -v --columns=pid,application";

    private string GetExecutablePathCommand(string appName)
        => $"""
            #!/bin/bash
            tmp_path="$(flatpak info -l {appName})/files/bin"
            echo "$tmp_path/$(ls -S "$tmp_path" | head -n1)"
            """;

    private IEnumerable<ProcessInfo> GetFlatpakApps()
    {
        var processResult = LinuxUtils.RunShellScript(GetFlatpakRunningApps);
        if (!processResult) return []; // add logs

        var applications = processResult
            .Value.Trim()
            .Split(Environment.NewLine)
            .Select(x =>
            {
                var info = x.Trim().Split();
                if (info.Length != 2) return null;
                
                var pid = int.Parse(info[0]);
                var appName = info[1];
                
                var locationResult =
                    LinuxUtils.RunShellScript(
                        GetExecutablePathCommand(appName));
                if (!locationResult) return null;
                
                return new ProcessInfo
                {
                    Pid = pid,
                    Name = appName,
                    ExecutablePath = locationResult.Value.Trim(),
                };
            })
            .WhereNotNull();

        return applications;
    }

    public IEnumerable<ProcessInfo> Invoke(IEnumerable<ProcessInfo>? processInfos = null)
    {
        var apps = GetFlatpakApps();
        return processInfos is null 
            ? apps 
            : processInfos.Concat(apps);
    }
}