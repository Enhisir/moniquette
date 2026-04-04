using Moniquette.Common.Models;

namespace Moniquette.Elastic.Entities;

public class ElasticSession
{
    public Guid Id { get; set; }
    
    public string FirstName { get; set; } = null!;
    
    public string MiddleName { get; set; } = null!;
    
    public string LastName { get; set; } = null!;
    
    public HardwareBriefInfo HardwareInfo { get; set; } = null!;
}