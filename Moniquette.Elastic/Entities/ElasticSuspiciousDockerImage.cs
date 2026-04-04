namespace Moniquette.Elastic.Entities;

public class ElasticSuspiciousDockerImage
{
    public string Name { get; set; } = null!;
    public string ImageName { get; set; } = null!;
    public string ImageDigest { get; set; } = null!;
}