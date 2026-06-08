using Elastic.Clients.Elasticsearch;
using Moniquette.Common.Models;
using Moniquette.Elastic.Entities;
using Moniquette.Elastic.Infrastructure;

namespace Moniquette.Server.Repositories;

public interface ISuspiciousDockerImageRepository
{
    Task SaveAsync(ElasticSuspiciousDockerImage image, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<ElasticSuspiciousDockerImage>> FindMatchesAsync(
        RunningDockerContainer container,
        CancellationToken cancellationToken);
}

public class ElasticSuspiciousDockerImageRepository(
    ElasticsearchClient client,
    ILogger<ElasticSuspiciousDockerImageRepository> logger) : ISuspiciousDockerImageRepository
{
    public async Task SaveAsync(ElasticSuspiciousDockerImage image, CancellationToken cancellationToken)
    {
        image.ImageName = NormalizeImageName(image.ImageName);
        image.ImageDigest = string.IsNullOrWhiteSpace(image.ImageDigest)
            ? null
            : image.ImageDigest.Trim();

        var response = await client.IndexAsync(
            image,
            descriptor => descriptor
                .Index(ElasticIndexNames.SuspiciousDockerImages)
                .Id(image.Id),
            cancellationToken);

        if (!response.IsValidResponse)
        {
            logger.LogError(
                "Failed to save suspicious Docker image {ImageName}: {DebugInformation}",
                image.ImageName,
                response.DebugInformation);
            throw new InvalidOperationException("Failed to save suspicious Docker image.");
        }
    }

    public async Task<IReadOnlyCollection<ElasticSuspiciousDockerImage>> FindMatchesAsync(
        RunningDockerContainer container,
        CancellationToken cancellationToken)
    {
        var imageName = NormalizeImageName(container.ImageName);
        var imageDigest = string.IsNullOrWhiteSpace(container.ImageDigest)
            ? null
            : container.ImageDigest.Trim();

        var response = imageDigest is null
            ? await client.SearchAsync<ElasticSuspiciousDockerImage>(
                descriptor => descriptor
                    .Indices(ElasticIndexNames.SuspiciousDockerImages)
                    .Size(20)
                    .Query(q => q.Term(t => t.Field(i => i.ImageName).Value(imageName))),
                cancellationToken)
            : await client.SearchAsync<ElasticSuspiciousDockerImage>(
                descriptor => descriptor
                    .Indices(ElasticIndexNames.SuspiciousDockerImages)
                    .Size(20)
                    .Query(q => q.Bool(b => b
                        .Should(
                            s => s.Term(t => t.Field(i => i.ImageName).Value(imageName)),
                            s => s.Term(t => t.Field(i => i.ImageDigest).Value(imageDigest)))
                        .MinimumShouldMatch(1))),
                cancellationToken);

        if (!response.IsValidResponse)
        {
            logger.LogError(
                "Failed to search suspicious Docker image {ImageName}: {DebugInformation}",
                container.ImageName,
                response.DebugInformation);
            throw new InvalidOperationException("Failed to search suspicious Docker images.");
        }

        return response.Documents;
    }

    private static string NormalizeImageName(string imageName)
        => imageName.Trim().ToLowerInvariant();
}
