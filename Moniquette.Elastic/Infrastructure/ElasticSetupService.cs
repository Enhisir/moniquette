using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.IndexManagement;
using Moniquette.Common.Models;
using Moniquette.Elastic.Entities;
using Moniquette.Elastic.Extensions;

namespace Moniquette.Elastic.Infrastructure;

public class ElasticSetupService(ElasticsearchClient client)
{
    private const string SessionIndexName = "sessions";
    private const string ReportIndexName = "reports";

    public async Task SetupAsync()
    {
        await CreateIndexAsync(SessionIndexName, SetupMappingsSessionIndex);
        await CreateIndexAsync(ReportIndexName, SetupMappingsReportsIndex);
    }

    private async Task CreateIndexAsync(string indexName, Action<CreateIndexRequestDescriptor> mapping)
    {
        var exists = await client.Indices.ExistsAsync(indexName);
        if (exists.Exists) return;

        var response = await client.Indices.CreateAsync(indexName, mapping.Invoke);

        if (!response.Acknowledged)
        {
            throw new Exception($"Failed to create '{indexName}' index");
        }
    }

    private void SetupMappingsSessionIndex(CreateIndexRequestDescriptor descriptor)
    {
        descriptor.Mappings(m => m
            .Properties<ElasticSession>(pd =>
                {
                    pd.Keyword(s => s.Id);
                    pd.Text(
                        s => s.FirstName,
                        desc => desc.Fields(f => f.Keyword("keyword")));
                    pd.Text(
                        s => s.MiddleName,
                        desc => desc.Fields(f => f.Keyword("keyword")));
                    pd.Text(
                        s => s.LastName,
                        desc => desc.Fields(f => f.Keyword("keyword")));
                    pd.Object(p => p.HardwareInfo, o => o.Enabled(false));
                }
            )
        ).WithDefaults();
    }

    private void SetupMappingsReportsIndex(CreateIndexRequestDescriptor descriptor)
    {
        descriptor.Mappings(md =>
            md.Properties<ElasticReport>(pd =>
            {
                pd.Keyword(p => p.SessionId);
                pd.Date(p => p.Timestamp);
                pd.Keyword(p => p.ProcessIds);
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
                        .Keyword(d => d.DockerContainers.First().ImageDigest)
                        .Keyword(d => d.DockerContainers.First().ImageDigest)
                    )
                );
            })
        ).WithDefaults();
    }
    
    private void SetupMappings_Index(CreateIndexRequestDescriptor descriptor)
    {
        /*
        descriptor.Mappings(m => m
            .Properties<_>(pd =>
                {
                }
            )
        ).WithDefaults();
        */
        throw new NotImplementedException();
    }

    public async Task CreateProcessIndexAsync()
    {
        throw new NotImplementedException();
    }

    public async Task CreateRemarksIndexAsync()
    {
        throw new NotImplementedException();
    }

    public async Task CreateSuspiciousProcessIndexesAsync()
    {
    }
}