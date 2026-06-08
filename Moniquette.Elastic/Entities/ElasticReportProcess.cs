namespace Moniquette.Elastic.Entities;

public class ElasticReportProcess
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid SessionId { get; set; }

    public Guid ReportId { get; set; }

    public DateTime Timestamp { get; set; }

    public int Pid { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Title { get; set; }

    public string ExecutablePath { get; set; } = string.Empty;

    public long[] Signature { get; set; } = [];

    public long[] Bands { get; set; } = [];
}
