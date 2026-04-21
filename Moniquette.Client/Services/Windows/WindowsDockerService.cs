using System.Diagnostics;
using Moniquette.Client.Services.Abstractions;
using Moniquette.Common.Models;

namespace Moniquette.Client.Services.Windows;

public class WindowsDockerService : IDockerService
{
    private const string CheckDockerWindowsCommand = "Get-Process 'com.docker.proxy'";

    private const string GetContainersWindowsCommand =
        """
        Not Implemented
        """;

    public bool CheckDockerIsRunning()
    {
        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"-ExecutionPolicy Bypass -Command \"{CheckDockerWindowsCommand}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var proc = Process.Start(psi);
            return proc?.ExitCode == 0;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public List<RunningDockerContainer> GetRunningDockerContainers()
    {
        throw new NotImplementedException();
    }
}