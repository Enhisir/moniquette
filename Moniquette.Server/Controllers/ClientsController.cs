using Microsoft.AspNetCore.Mvc;
using Moniquette.Common.Dto;
using Moniquette.Common.Models;
using Moniquette.Server.Repositories;

namespace Moniquette.Server.Controllers;

[ApiController]
[Route("api/v2/sessions")]
public class ClientsController(
    ISessionRepository sessionRepository,
    IReportRepository reportRepository,
    IThreatRepository threatRepository) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<SessionStateDto>>> GetSessions(
        CancellationToken cancellationToken)
    {
        var sessions = await sessionRepository.GetAllAsync(cancellationToken);
        var result = new List<SessionStateDto>();

        foreach (var session in sessions)
        {
            result.Add(await CreateSessionStateAsync(session, cancellationToken));
        }

        return Ok(result);
    }

    [HttpGet("{sessionId:guid}")]
    public async Task<ActionResult<SessionStateDto>> GetSession(
        Guid sessionId,
        CancellationToken cancellationToken)
    {
        var session = await sessionRepository.GetByIdAsync(sessionId, cancellationToken);
        if (session is null)
        {
            return NotFound();
        }

        return Ok(await CreateSessionStateAsync(session, cancellationToken));
    }

    [HttpGet("{sessionId:guid}/reports")]
    public async Task<ActionResult<IReadOnlyCollection<ReportDto>>> GetSessionReports(
        Guid sessionId,
        [FromQuery] int limit,
        CancellationToken cancellationToken)
    {
        if (!await sessionRepository.ExistsAsync(sessionId, cancellationToken))
        {
            return NotFound();
        }

        var reports = await reportRepository.GetBySessionIdAsync(
            sessionId,
            NormalizeLimit(limit, defaultValue: 20, max: 200),
            cancellationToken);

        return Ok(reports.Select(MapReport).ToList());
    }

    [HttpGet("{sessionId:guid}/threats")]
    public async Task<ActionResult<IReadOnlyCollection<ThreatDto>>> GetSessionThreats(
        Guid sessionId,
        [FromQuery] int limit,
        CancellationToken cancellationToken)
    {
        if (!await sessionRepository.ExistsAsync(sessionId, cancellationToken))
        {
            return NotFound();
        }

        var threats = await threatRepository.GetBySessionIdAsync(
            sessionId,
            NormalizeLimit(limit, defaultValue: 100, max: 500),
            cancellationToken);

        return Ok(threats.Select(MapThreat).ToList());
    }

    private async Task<SessionStateDto> CreateSessionStateAsync(
        Session session,
        CancellationToken cancellationToken)
    {
        var lastReport = await reportRepository.GetLatestBySessionIdAsync(session.Id, cancellationToken);
        var threats = lastReport is null
            ? []
            : await threatRepository.GetByReportIdAsync(lastReport.Id, limit: 100, cancellationToken);

        return new SessionStateDto
        {
            Session = session,
            LastReport = lastReport is null ? null : MapReport(lastReport),
            Threats = threats.Select(MapThreat).ToList()
        };
    }

    private static int NormalizeLimit(int limit, int defaultValue, int max)
    {
        if (limit <= 0)
        {
            return defaultValue;
        }

        return Math.Min(limit, max);
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
