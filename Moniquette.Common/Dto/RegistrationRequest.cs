namespace Moniquette.Common.Dto;

public class RegistrationRequest
{
    public string FirstName { get; set; } = null!;
    public string MiddleName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string SerialNumber { get; set; } = null!;
}