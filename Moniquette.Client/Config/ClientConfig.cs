using System.ComponentModel.DataAnnotations;
using Moniquette.Common.Dto;

namespace Moniquette.Client.Config;

public class ClientConfig
{
    [Required] public string BaseAddress { get; set; } = null!;
    [Required] public string AuthorityKeyPath { get; set; } = null!;
    
    [Required] public RegistrationRequest UserInfo { get; set; } // must leave config
}