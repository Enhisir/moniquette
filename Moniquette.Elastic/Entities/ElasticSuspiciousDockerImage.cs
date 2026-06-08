namespace Moniquette.Elastic.Entities;

public class ElasticSuspiciousDockerImage
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = null!;

    public string ImageName { get; set; } = null!;

    public string? ImageDigest { get; set; }

    public string Details { get; set; } = string.Empty;
}
