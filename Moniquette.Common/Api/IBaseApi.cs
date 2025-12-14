using Moniquette.Common.Api.Result;
using Moniquette.Common.Dto;
using Moniquette.Common.Models;

namespace Moniquette.Common.Api;

public interface IBaseApi
{
    public Task<IApiResult> RegisterAsync(RegistrationRequestDto requestDto, CancellationToken cancellationToken = default);
    public Task<IApiResult> SendReportAsync(Report report, CancellationToken cancellationToken = default);
}