using System.Diagnostics;
using System.Runtime.Versioning;
using System.Text;
using Moniquette.Common.Models;
using Moniquette.Common.Utils;

namespace Moniquette.ProcessObserver.ProcessPipeline;

[SupportedOSPlatform(Literals.Windows)]
public class WindowsProcessHandler : IProcessHandler
{
    public IEnumerable<ProcessInfo> Invoke(IEnumerable<ProcessInfo>? processInfos = null)
    {
        var previousProcesses = processInfos?.ToList() ?? [];
        var previousPids = previousProcesses
            .Select(process => process.Pid)
            .ToHashSet();
        var knownExecutablePaths = previousProcesses
            .Select(process => process.ExecutablePath)
            .Where(path => !string.IsNullOrWhiteSpace(path))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var windowTitles = GetWindowTitlesByPid();
        var windowsProcesses = Process
            .GetProcesses()
            .Where(process => !previousPids.Contains(process.Id))
            .Select(process => TryCreateProcessInfo(process, windowTitles))
            .Where(process => process is not null)
            .Select(process => process!)
            .Where(process => knownExecutablePaths.Add(process.ExecutablePath))
            .ToList();

        return previousProcesses.Concat(windowsProcesses);
    }

    private static ProcessInfo? TryCreateProcessInfo(
        Process process,
        IReadOnlyDictionary<int, string> windowTitles)
    {
        try
        {
            var executablePath = process.MainModule?.FileName;
            if (string.IsNullOrWhiteSpace(executablePath) || !File.Exists(executablePath))
            {
                return null;
            }

            windowTitles.TryGetValue(process.Id, out var title);
            if (string.IsNullOrWhiteSpace(title) && !string.IsNullOrWhiteSpace(process.MainWindowTitle))
            {
                title = process.MainWindowTitle;
            }

            return new ProcessInfo
            {
                Pid = process.Id,
                Name = process.ProcessName,
                Title = string.IsNullOrWhiteSpace(title) ? null : title,
                ExecutablePath = executablePath
            };
        }
        catch
        {
            // Доступ к MainModule часто запрещен для системных процессов и процессов другой разрядности.
            return null;
        }
    }

    private static Dictionary<int, string> GetWindowTitlesByPid()
    {
        var titles = new Dictionary<int, string>();
        WindowsUtils.EnumWindows((hWnd, lParam) =>
        {
            if (!WindowsUtils.IsWindowVisible(hWnd))
            {
                return true;
            }

            var titleLength = WindowsUtils.GetWindowTextLength(hWnd);
            if (titleLength == 0)
            {
                return true;
            }

            var titleBuffer = new StringBuilder(titleLength + 1);
            _ = WindowsUtils.GetWindowText(hWnd, titleBuffer, titleBuffer.Capacity);
            var title = titleBuffer.ToString().Trim();
            if (string.IsNullOrWhiteSpace(title))
            {
                return true;
            }

            WindowsUtils.GetWindowThreadProcessId(hWnd, out var processId);
            titles.TryAdd((int)processId, title);
            return true;
        }, IntPtr.Zero);

        return titles;
    }
}
