using Moniquette.Common.Enums;

namespace Moniquette.Common.Models;

public record struct Threat()
{
    public Guid Id { get; init; } = Guid.NewGuid();
    
    public DateTime Timestamp { get; init; } =  DateTime.UtcNow;
    
    public required ThreatType Type { get; init; } = ThreatType.Unknown;
    
    public required Guid SessionId { get; init; }
    
    public required Guid ReportId { get; init; }
    
    public required string Details { get; init; }
}
