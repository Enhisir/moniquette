using Elastic.Clients.Elasticsearch;
using Moniquette.Common.Models;
using Moniquette.Elastic.Entities;
using Moniquette.Elastic.Infrastructure;
using Moniquette.Elastic.Services;

namespace Moniquette.Server.Repositories;

public interface IReportRepository
{
    Task SaveAsync(Report report, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Report>> GetBySessionIdAsync(
        Guid sessionId,
        int limit,
        CancellationToken cancellationToken);

    Task<Report?> GetLatestBySessionIdAsync(Guid sessionId, CancellationToken cancellationToken);
}

public class ElasticReportRepository(
    ElasticsearchClient client,
    IProcessBandService processBandService,
    ILogger<ElasticReportRepository> logger) : IReportRepository
{
    public async Task SaveAsync(Report report, CancellationToken cancellationToken)
    {
        var processes = report.Processes
            .Select(process => processBandService.CreateReportProcess(
                process,
                report.SessionId,
                report.Id,
                report.Timestamp))
            .ToList();

        var document = new ElasticReport
        {
            Id = report.Id,
            SessionId = report.SessionId,
            Timestamp = report.Timestamp,
            ProcessIds = processes.Select(p => p.Id).ToList(),
            Processes = processes,
            HardwareInfo = report.HardwareInfo,
            Connections = report.Connections,
            WindowsRegistry = report.WindowsRegistry,
            IsDockerEnabled = report.IsDockerEnabled,
            DockerContainers = report.DockerContainers
        };

        var response = await client.IndexAsync(
            document,
            descriptor => descriptor
                .Index(ElasticIndexNames.Reports)
                .Id(report.Id),
            cancellationToken);

        if (!response.IsValidResponse)
        {
            logger.LogError("Failed to save report {ReportId} to Elasticsearch: {DebugInformation}",
                report.Id,
                response.DebugInformation);
            throw new InvalidOperationException("Failed to save report.");
        }

        foreach (var process in processes)
        {
            var processResponse = await client.IndexAsync(
                process,
                descriptor => descriptor
                    .Index(ElasticIndexNames.Processes)
                    .Id(process.Id),
                cancellationToken);

            if (!processResponse.IsValidResponse)
            {
                logger.LogError("Failed to save report process {ProcessId} for report {ReportId}: {DebugInformation}",
                    process.Id,
                    report.Id,
                    processResponse.DebugInformation);
                throw new InvalidOperationException("Failed to save report process.");
            }
        }
    }

    public async Task<IReadOnlyCollection<Report>> GetBySessionIdAsync(
        Guid sessionId,
        int limit,
        CancellationToken cancellationToken)
    {
        var response = await client.SearchAsync<ElasticReport>(
            descriptor => descriptor
                .Indices(ElasticIndexNames.Reports)
                .Size(limit)
                .Query(q => q.Term(t => t.Field(r => r.SessionId).Value(sessionId.ToString())))
                .Sort(s => s.Field(f => f.Field(r => r.Timestamp).Order(SortOrder.Desc))),
            cancellationToken);

        if (!response.IsValidResponse)
        {
            logger.LogError("Failed to read reports for session {SessionId}: {DebugInformation}",
                sessionId,
                response.DebugInformation);
            throw new InvalidOperationException("Failed to read reports.");
        }

        return response.Documents
            .Select(Map)
            .ToList();
    }

    public async Task<Report?> GetLatestBySessionIdAsync(Guid sessionId, CancellationToken cancellationToken)
        => (await GetBySessionIdAsync(sessionId, 1, cancellationToken)).FirstOrDefault();

    private static Report Map(ElasticReport report)
        => new()
        {
            Id = report.Id,
            SessionId = report.SessionId,
            Timestamp = report.Timestamp,
            Processes = report.Processes
                .Select(process => new ProcessInfo
                {
                    Pid = process.Pid,
                    Name = process.Name,
                    Title = process.Title,
                    ExecutablePath = process.ExecutablePath,
                    Signature = process.Signature
                })
                .ToList(),
            HardwareInfo = report.HardwareInfo,
            Connections = report.Connections,
            WindowsRegistry = report.WindowsRegistry,
            IsDockerEnabled = report.IsDockerEnabled,
            DockerContainers = report.DockerContainers
        };
}
