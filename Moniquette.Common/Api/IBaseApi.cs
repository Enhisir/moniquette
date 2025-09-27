using Moniquette.Common.Api.Result;
using Moniquette.Common.Dto;
using Moniquette.Common.Models;

namespace Moniquette.Common.Api;

public interface IBaseApi
{
    public Task<IApiResult> Register(RegistrationRequest request);
    public Task<IApiResult> SendReport(Report report);
}