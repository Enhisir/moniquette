namespace Moniquette.Common.Models;

public class Session
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string MiddleName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string SerialNumber { get; set; } = null!;
}