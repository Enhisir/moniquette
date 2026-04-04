namespace Moniquette.Elastic.Entities;

public class ElasticProcessInfo
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public int Pid { get; set; }
    
    public string Name { get; set; } = null!;
    
    public string? Title { get; set; }
    
    public string ExecutablePath { get; set; } = string.Empty;

    public long[] Signature { get; set; } = null!;
    
    public float[] Bands { get; set; } = null!;
}