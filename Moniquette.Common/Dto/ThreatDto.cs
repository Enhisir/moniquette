using Moniquette.Common.Enums;

namespace Moniquette.Common.Dto;

public class ThreatDto
{
    public Guid Id { get; set; }

    public DateTime Timestamp { get; set; }

    public ThreatType Type { get; set; } = ThreatType.Unknown;

    public Guid SessionId { get; set; }

    public Guid ReportId { get; set; }

    public string Details { get; set; } = string.Empty;
}
