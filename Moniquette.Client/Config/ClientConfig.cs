using System.ComponentModel.DataAnnotations;
using Moniquette.Common.Dto;

namespace Moniquette.Client.Config;

public class ClientConfig
{
    [Required] public string BaseHttpAddress { get; set; } = null!;
    [Required] public string BaseGrpcAddress { get; set; } = null!;
    [Required] public string AuthorityKeyPath { get; set; } = null!;
    [Required] public int ReportDelayMs { get; set; }
    [Required] public RegistrationRequestDto UserInfo { get; set; } = null!; // must leave config
}