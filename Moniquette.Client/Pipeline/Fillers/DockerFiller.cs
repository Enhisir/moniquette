using System.Diagnostics;
using Hardware.Info;
using Moniquette.Common.Models;
using Moniquette.Common.Utils;

namespace Moniquette.Client.Pipeline.Fillers;

public class DockerFiller(
    IHardwareInfo hardwareInfo) : IReportFiller
{
    private const string CheckDockerWindowsCommand = "Get-Process 'com.docker.proxy'";
    private const string CheckDockerLinuxCommand = "systemctl is-active docker";
    private const string GetContainersCommand = "docker ps --format \"table {{.ID}}\\t{{.Image}}\\t{{.Names}}\"";
    private const string GetContainersLinuxCommand = $"{GetContainersCommand} | tail -n +2";

    public Task<Report> Fill(Report request, CancellationToken cancellationToken)
    {
        switch (GetOperatingSystemName())
        {
            case Literals.Windows:
                if (IsDockerRunningOnWindows())
                {
                    FillReportWithContainers(request);
                }

                break;
            case Literals.Linux:
                var processResult = LinuxUtils.RunShellScript(CheckDockerLinuxCommand);
                if (processResult && processResult.Value.Equals("active"))
                {
                    FillReportWithContainers(request);
                }

                break;
            case Literals.MacOS:
                break;
        }

        return Task.FromResult(request);
    }

    private string GetOperatingSystemName()
    {
        var fullname = hardwareInfo.OperatingSystem.Name.ToLower();
        if (fullname.Contains("windows")) return Literals.Windows;
        if (fullname.Contains("linux")) return Literals.Linux;
        if (fullname.Contains("macos")) return Literals.MacOS;
        return fullname;
    }

    private static void FillReportWithContainers(Report report)
    {
        var processResult = LinuxUtils.RunShellScript(GetContainersLinuxCommand);
        if (!processResult)
        {
            throw new Exception(
                $"Couldn't execute the command '{GetContainersCommand}'{Environment.NewLine}" 
                + processResult.Error);
        }
        var output = processResult.Value.Trim();
        report.IsDockerEnabled = true;
        report.DockerContainers =
            output.Equals(string.Empty)
                ? []
                : output
                    .Split(Environment.NewLine)
                    .Select(line =>
                    {
                        var info = line.Trim().Split();
                        return new RunningDockerContainer
                        {
                            Id = info[0],
                            Name = info[1],
                            ImageName = info[2]
                        };
                    }).ToList();
    }

    private static bool IsDockerRunningOnWindows()
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
}