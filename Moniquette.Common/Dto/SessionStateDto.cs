using Moniquette.Common.Models;

namespace Moniquette.Common.Dto;

public class SessionStateDto
{
    public Session Session { get; set; } = null!;

    public ReportDto? LastReport { get; set; }

    public List<ThreatDto> Threats { get; set; } = [];
}
