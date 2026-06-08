using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Moniquette.Common.Models;
using Moniquette.Elastic.Entities;
using Moniquette.Elastic.Infrastructure;
using Moniquette.Elastic.Services;

namespace Moniquette.Server.Repositories;

public enum ProcessCatalogPlatform
{
    Windows,
    Linux
}

public interface ISuspiciousProcessRepository
{
    Task SaveAsync(
        ProcessInfo process,
        ProcessCatalogPlatform platform,
        CancellationToken cancellationToken);

    Task<IReadOnlyCollection<ElasticProcessInfo>> FindByBandsAsync(
        long[] bands,
        ProcessCatalogPlatform platform,
        int limit,
        CancellationToken cancellationToken);
}

public class ElasticSuspiciousProcessRepository(
    ElasticsearchClient client,
    IProcessBandService processBandService,
    ILogger<ElasticSuspiciousProcessRepository> logger) : ISuspiciousProcessRepository
{
    public async Task SaveAsync(
        ProcessInfo process,
        ProcessCatalogPlatform platform,
        CancellationToken cancellationToken)
    {
        var document = new ElasticProcessInfo
        {
            Pid = process.Pid,
            Name = process.Name,
            Title = process.Title,
            ExecutablePath = process.ExecutablePath,
            Signature = process.Signature ?? [],
            Bands = processBandService.CreateBands(process.Signature)
        };

        var response = await client.IndexAsync(
            document,
            descriptor => descriptor
                .Index(GetIndexName(platform))
                .Id(document.Id),
            cancellationToken);

        if (!response.IsValidResponse)
        {
            logger.LogError(
                "Failed to save suspicious process {ProcessName} to Elasticsearch: {DebugInformation}",
                process.Name,
                response.DebugInformation);
            throw new InvalidOperationException("Failed to save suspicious process.");
        }
    }

    public async Task<IReadOnlyCollection<ElasticProcessInfo>> FindByBandsAsync(
        long[] bands,
        ProcessCatalogPlatform platform,
        int limit,
        CancellationToken cancellationToken)
    {
        if (bands.Length == 0)
        {
            return [];
        }

        var values = bands
            .Distinct()
            .Select(FieldValue.Long)
            .ToArray();

        var response = await client.SearchAsync<ElasticProcessInfo>(
            descriptor => descriptor
                .Indices(GetIndexName(platform))
                .Size(limit)
                .Query(q => q.TermsSet(t => t
                    .Field(p => p.Bands)
                    .Terms(values)
                    .MinimumShouldMatch(1))),
            cancellationToken);

        if (!response.IsValidResponse)
        {
            logger.LogError(
                "Failed to search suspicious processes by bands in {Platform} catalog: {DebugInformation}",
                platform,
                response.DebugInformation);
            throw new InvalidOperationException("Failed to search suspicious processes.");
        }

        return response.Documents;
    }

    private static string GetIndexName(ProcessCatalogPlatform platform)
        => platform switch
        {
            ProcessCatalogPlatform.Windows => ElasticIndexNames.SuspiciousProcessesWindows,
            ProcessCatalogPlatform.Linux => ElasticIndexNames.SuspiciousProcessesLinux,
            _ => throw new ArgumentOutOfRangeException(nameof(platform), platform, null)
        };
}
