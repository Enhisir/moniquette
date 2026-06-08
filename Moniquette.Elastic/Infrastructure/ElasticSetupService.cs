using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.IndexManagement;
using Moniquette.Elastic.Entities;
using Moniquette.Elastic.Extensions;

namespace Moniquette.Elastic.Infrastructure;

public class ElasticSetupService(ElasticsearchClient client)
{
    public async Task SetupAsync(CancellationToken cancellationToken = default)
    {
        await CreateIndexAsync(ElasticIndexNames.Sessions, SetupMappingsSessionIndex, cancellationToken);
        await CreateIndexAsync(ElasticIndexNames.Reports, SetupMappingsReportsIndex, cancellationToken);
        await CreateIndexAsync(ElasticIndexNames.Processes, SetupMappingsReportProcessesIndex, cancellationToken);
        await CreateIndexAsync(ElasticIndexNames.Threats, SetupMappingsThreatsIndex, cancellationToken);
        await CreateIndexAsync(ElasticIndexNames.SuspiciousProcessesWindows, SetupMappingsSuspiciousProcessesIndex, cancellationToken);
        await CreateIndexAsync(ElasticIndexNames.SuspiciousProcessesLinux, SetupMappingsSuspiciousProcessesIndex, cancellationToken);
        await CreateIndexAsync(ElasticIndexNames.SuspiciousDockerImages, SetupMappingsSuspiciousDockerImagesIndex, cancellationToken);
    }

    private async Task CreateIndexAsync(
        string indexName,
        Action<CreateIndexRequestDescriptor> mapping,
        CancellationToken cancellationToken)
    {
        var exists = await client.Indices.ExistsAsync(indexName, cancellationToken);
        if (exists.Exists) return;

        var response = await client.Indices.CreateAsync(indexName, mapping.Invoke, cancellationToken);
        if (!response.Acknowledged)
        {
            throw new InvalidOperationException($"Failed to create '{indexName}' index.");
        }
    }

    private void SetupMappingsSessionIndex(CreateIndexRequestDescriptor descriptor)
    {
        descriptor.Mappings(m => m
            .Properties<ElasticSession>(pd =>
            {
                pd.Keyword(s => s.Id);
                pd.Text(s => s.FirstName, desc => desc.Fields(f => f.Keyword("keyword")));
                pd.Text(s => s.MiddleName, desc => desc.Fields(f => f.Keyword("keyword")));
                pd.Text(s => s.LastName, desc => desc.Fields(f => f.Keyword("keyword")));
                pd.Object(p => p.HardwareInfo, o => o.Enabled(false));
            })
        ).WithDefaults();
    }

    private void SetupMappingsReportsIndex(CreateIndexRequestDescriptor descriptor)
    {
        descriptor.Mappings(md =>
            md.Properties<ElasticReport>(pd =>
            {
                pd.Keyword(p => p.Id);
                pd.Keyword(p => p.SessionId);
                pd.Date(p => p.Timestamp);
                pd.Keyword(p => p.ProcessIds);
                pd.Nested(p => p.Processes, n => n
                    .Properties(np => np
                        .Keyword(p => p.Processes.First().Id)
                        .IntegerNumber(p => p.Processes.First().Pid)
                        .Keyword(p => p.Processes.First().Name)
                        .Text(p => p.Processes.First().Title)
                        .Keyword(p => p.Processes.First().ExecutablePath)
                        .LongNumber(p => p.Processes.First().Signature, desc => desc.Index(false))
                        .LongNumber(p => p.Processes.First().Bands)
                    )
                );
                pd.Object(p => p.HardwareInfo, o => o.Enabled(false));
                pd.Nested(p => p.Connections, n => n
                    .Properties(np => np
                        .Keyword(c => c.Connections.First().Name)
                        .Keyword(c => c.Connections.First().Description)
                        .IntegerNumber(c => c.Connections.First().InterfaceType)
                        .Keyword(c => c.Connections.First().IpAddressString)
                        .Keyword(c => c.Connections.First().MacAddressString)
                        .Keyword(c => c.Connections.First().DomainNameServices)
                        .Keyword(c => c.Connections.First().Gateways)
                    )
                );
                pd.Object(p => p.WindowsRegistry, o => o.Enabled(false));
                pd.Boolean(p => p.IsDockerEnabled);
                pd.Nested(p => p.DockerContainers, n => n
                    .Properties(np => np
                        .Keyword(d => d.DockerContainers.First().Name)
                        .Keyword(d => d.DockerContainers.First().ImageName)
                        .Keyword(d => d.DockerContainers.First().ImageDigest)
                    )
                );
            })
        ).WithDefaults();
    }

    private void SetupMappingsThreatsIndex(CreateIndexRequestDescriptor descriptor)
    {
        descriptor.Mappings(m => m
            .Properties<ElasticThreat>(pd =>
            {
                pd.Keyword(t => t.Id);
                pd.Date(t => t.Timestamp);
                pd.Keyword(t => t.Type);
                pd.Keyword(t => t.SessionId);
                pd.Keyword(t => t.ReportId);
                pd.Text(t => t.Details);
            })
        ).WithDefaults();
    }

    private void SetupMappingsReportProcessesIndex(CreateIndexRequestDescriptor descriptor)
    {
        descriptor.Mappings(m => m
            .Properties<ElasticReportProcess>(pd =>
            {
                pd.Keyword(p => p.Id);
                pd.Keyword(p => p.SessionId);
                pd.Keyword(p => p.ReportId);
                pd.Date(p => p.Timestamp);
                pd.IntegerNumber(p => p.Pid);
                pd.Keyword(p => p.Name);
                pd.Text(p => p.Title);
                pd.Keyword(p => p.ExecutablePath);
                pd.LongNumber(p => p.Signature, desc => desc.Index(false));
                pd.LongNumber(p => p.Bands);
            })
        ).WithDefaults();
    }

    private void SetupMappingsSuspiciousProcessesIndex(CreateIndexRequestDescriptor descriptor)
    {
        descriptor.Mappings(m => m
            .Properties<ElasticProcessInfo>(pd =>
            {
                pd.Keyword(p => p.Id);
                pd.IntegerNumber(p => p.Pid);
                pd.Keyword(p => p.Name);
                pd.Text(p => p.Title);
                pd.Keyword(p => p.ExecutablePath);
                pd.LongNumber(p => p.Signature, desc => desc.Index(false));
                pd.LongNumber(p => p.Bands);
            })
        ).WithDefaults();
    }

    private void SetupMappingsSuspiciousDockerImagesIndex(CreateIndexRequestDescriptor descriptor)
    {
        descriptor.Mappings(m => m
            .Properties<ElasticSuspiciousDockerImage>(pd =>
            {
                pd.Keyword(i => i.Id);
                pd.Keyword(i => i.Name);
                pd.Keyword(i => i.ImageName);
                pd.Keyword(i => i.ImageDigest);
                pd.Text(i => i.Details);
            })
        ).WithDefaults();
    }
}
