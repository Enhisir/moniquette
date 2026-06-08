using Elastic.Clients.Elasticsearch;
using Moniquette.Common.Models;
using Moniquette.Elastic.Entities;
using Moniquette.Elastic.Infrastructure;

namespace Moniquette.Server.Repositories;

public interface IThreatRepository
{
    Task SaveAsync(Threat threat, CancellationToken cancellationToken);

    Task SaveManyAsync(IReadOnlyCollection<Threat> threats, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Threat>> GetBySessionIdAsync(
        Guid sessionId,
        int limit,
        CancellationToken cancellationToken);
}

public class ElasticThreatRepository(
    ElasticsearchClient client,
    ILogger<ElasticThreatRepository> logger) : IThreatRepository
{
    public async Task SaveAsync(Threat threat, CancellationToken cancellationToken)
    {
        var document = Map(threat);
        var response = await client.IndexAsync(
            document,
            descriptor => descriptor
                .Index(ElasticIndexNames.Threats)
                .Id(threat.Id),
            cancellationToken);

        if (!response.IsValidResponse)
        {
            logger.LogError(
                "Failed to save threat {ThreatId} to Elasticsearch: {DebugInformation}",
                threat.Id,
                response.DebugInformation);
            throw new InvalidOperationException("Failed to save threat.");
        }
    }

    public async Task SaveManyAsync(IReadOnlyCollection<Threat> threats, CancellationToken cancellationToken)
    {
        foreach (var threat in threats)
        {
            await SaveAsync(threat, cancellationToken);
        }
    }

    public async Task<IReadOnlyCollection<Threat>> GetBySessionIdAsync(
        Guid sessionId,
        int limit,
        CancellationToken cancellationToken)
    {
        var response = await client.SearchAsync<ElasticThreat>(
            descriptor => descriptor
                .Indices(ElasticIndexNames.Threats)
                .Size(limit)
                .Query(q => q.Term(t => t.Field(threat => threat.SessionId).Value(sessionId.ToString())))
                .Sort(s => s.Field(f => f.Field(threat => threat.Timestamp).Order(SortOrder.Desc))),
            cancellationToken);

        if (!response.IsValidResponse)
        {
            logger.LogError("Failed to read threats for session {SessionId}: {DebugInformation}",
                sessionId,
                response.DebugInformation);
            throw new InvalidOperationException("Failed to read threats.");
        }

        return response.Documents
            .Select(Map)
            .ToList();
    }

    private static ElasticThreat Map(Threat threat)
        => new()
        {
            Id = threat.Id,
            Timestamp = threat.Timestamp,
            Type = threat.Type,
            SessionId = threat.SessionId,
            ReportId = threat.ReportId,
            Details = threat.Details
        };

    private static Threat Map(ElasticThreat threat)
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
