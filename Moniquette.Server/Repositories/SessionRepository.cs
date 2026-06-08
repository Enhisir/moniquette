using Elastic.Clients.Elasticsearch;
using Moniquette.Common.Models;
using Moniquette.Elastic.Entities;
using Moniquette.Elastic.Infrastructure;

namespace Moniquette.Server.Repositories;

public interface ISessionRepository
{
    Task SaveAsync(Session session, CancellationToken cancellationToken);
    
    Task<bool> ExistsAsync(Guid sessionId, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Session>> GetAllAsync(CancellationToken cancellationToken);

    Task<Session?> GetByIdAsync(Guid sessionId, CancellationToken cancellationToken);
}

public class ElasticSessionRepository(
    ElasticsearchClient client,
    ILogger<ElasticSessionRepository> logger) : ISessionRepository
{
    public async Task SaveAsync(Session session, CancellationToken cancellationToken)
    {
        var document = new ElasticSession
        {
            Id = session.Id,
            FirstName = session.FirstName,
            MiddleName = session.MiddleName,
            LastName = session.LastName,
            HardwareInfo = session.HardwareInfo
        };

        var response = await client.IndexAsync(
            document,
            descriptor => descriptor
                .Index(ElasticIndexNames.Sessions)
                .Id(session.Id),
            cancellationToken);

        if (!response.IsValidResponse)
        {
            logger.LogError("Failed to save session {SessionId} to Elasticsearch: {DebugInformation}",
                session.Id,
                response.DebugInformation);
            throw new InvalidOperationException("Failed to save session.");
        }
    }

    public async Task<bool> ExistsAsync(Guid sessionId, CancellationToken cancellationToken)
    {
        var response = await client.GetAsync<ElasticSession>(
            sessionId,
            descriptor => descriptor.Index(ElasticIndexNames.Sessions),
            cancellationToken);

        if (response.IsValidResponse)
        {
            return response.Found;
        }

        logger.LogWarning("Failed to check session {SessionId} in Elasticsearch: {DebugInformation}",
            sessionId,
            response.DebugInformation);
        return false;
    }

    public async Task<IReadOnlyCollection<Session>> GetAllAsync(CancellationToken cancellationToken)
    {
        var response = await client.SearchAsync<ElasticSession>(
            descriptor => descriptor
                .Indices(ElasticIndexNames.Sessions)
                .Size(1000),
            cancellationToken);

        if (!response.IsValidResponse)
        {
            logger.LogError("Failed to read sessions from Elasticsearch: {DebugInformation}",
                response.DebugInformation);
            throw new InvalidOperationException("Failed to read sessions.");
        }

        return response.Documents
            .Select(Map)
            .ToList();
    }

    public async Task<Session?> GetByIdAsync(Guid sessionId, CancellationToken cancellationToken)
    {
        var response = await client.GetAsync<ElasticSession>(
            sessionId,
            descriptor => descriptor.Index(ElasticIndexNames.Sessions),
            cancellationToken);

        if (response.Found && response.Source is not null)
        {
            return Map(response.Source);
        }

        if (!response.IsValidResponse)
        {
            logger.LogWarning("Failed to read session {SessionId} from Elasticsearch: {DebugInformation}",
                sessionId,
                response.DebugInformation);
        }

        return null;
    }

    private static Session Map(ElasticSession session)
        => new()
        {
            Id = session.Id,
            FirstName = session.FirstName,
            MiddleName = session.MiddleName,
            LastName = session.LastName,
            HardwareInfo = session.HardwareInfo
        };
}
