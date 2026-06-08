using Microsoft.AspNetCore.SignalR;
using Moniquette.Common.Dto;
using Moniquette.Common.Models;

namespace Moniquette.Server.Hubs;

public interface IMonitoringNotifier
{
    Task NotifyReportReceivedAsync(Report report, CancellationToken cancellationToken);

    Task NotifyReportAnalyzedAsync(
        Report report,
        IReadOnlyCollection<Threat> threats,
        CancellationToken cancellationToken);
}

public class SignalRMonitoringNotifier(
    IHubContext<MonitoringHub> hubContext,
    ILogger<SignalRMonitoringNotifier> logger) : IMonitoringNotifier
{
    public async Task NotifyReportReceivedAsync(Report report, CancellationToken cancellationToken)
    {
        var reportDto = MapReport(report);
        await SendToSessionGroupAsync(report.SessionId, MonitoringEvents.ReportReceived, reportDto, cancellationToken);
        await SendToSessionGroupAsync(report.SessionId, MonitoringEvents.SessionUpdated, reportDto, cancellationToken);
        await SendToSessionGroupAsync(report.SessionId, MonitoringEvents.HardwareUpdated, report.HardwareInfo, cancellationToken);
    }

    public async Task NotifyReportAnalyzedAsync(
        Report report,
        IReadOnlyCollection<Threat> threats,
        CancellationToken cancellationToken)
    {
        var threatDtos = threats.Select(MapThreat).ToList();
        var payload = new ReportAnalyzedEventDto
        {
            SessionId = report.SessionId,
            ReportId = report.Id,
            Threats = threatDtos
        };

        await SendToSessionGroupAsync(report.SessionId, MonitoringEvents.ReportAnalyzed, payload, cancellationToken);
        await SendToSessionGroupAsync(report.SessionId, MonitoringEvents.ThreatsUpdated, threatDtos, cancellationToken);
        await SendToSessionGroupAsync(report.SessionId, MonitoringEvents.SessionUpdated, payload, cancellationToken);
    }

    private async Task SendToSessionGroupAsync(
        Guid sessionId,
        string eventName,
        object payload,
        CancellationToken cancellationToken)
    {
        try
        {
            await hubContext
                .Clients
                .Group(MonitoringHubGroups.Session(sessionId))
                .SendAsync(eventName, payload, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogWarning(
                ex,
                "Failed to send SignalR event {EventName} for session {SessionId}.",
                eventName,
                sessionId);
        }
    }

    private static ReportDto MapReport(Report report)
        => new()
        {
            Id = report.Id,
            SessionId = report.SessionId,
            Timestamp = report.Timestamp,
            Processes = report.Processes,
            HardwareInfo = report.HardwareInfo,
            Connections = report.Connections,
            WindowsRegistry = report.WindowsRegistry,
            IsDockerEnabled = report.IsDockerEnabled,
            DockerContainers = report.DockerContainers
        };

    private static ThreatDto MapThreat(Threat threat)
        => new()
        {
            Id = threat.Id,
            Timestamp = threat.Timestamp,
            Type = threat.Type,
            SessionId = threat.SessionId,
            ReportId = threat.ReportId,
            Details = threat.Details
        };
}

public class ReportAnalyzedEventDto
{
    public Guid SessionId { get; set; }

    public Guid ReportId { get; set; }

    public List<ThreatDto> Threats { get; set; } = [];
}
