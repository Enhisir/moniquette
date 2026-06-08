namespace Moniquette.Elastic.Infrastructure;

public static class ElasticIndexNames
{
    public const string Sessions = "sessions";
    public const string Reports = "reports";
    public const string Threats = "threats";
    public const string Processes = "processes";
    public const string SuspiciousProcessesWindows = "suspicious_processes_windows";
    public const string SuspiciousProcessesLinux = "suspicious_processes_linux";
    public const string SuspiciousDockerImages = "suspicious_images_docker";
}
