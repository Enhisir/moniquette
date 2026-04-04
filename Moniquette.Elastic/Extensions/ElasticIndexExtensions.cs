using Elastic.Clients.Elasticsearch.IndexManagement;

namespace Moniquette.Elastic.Extensions;

public static class ElasticIndexExtensions
{
    public static CreateIndexRequestDescriptor WithDefaults(
        this CreateIndexRequestDescriptor descriptor)
    {
        return descriptor.Settings(s => s
            .NumberOfShards(1)
            .NumberOfReplicas(0)
        );
    }
}