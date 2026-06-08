using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using Moniquette.Client.Config;
using Moniquette.Common.Api;
using Moniquette.Common.Api.Result;
using Moniquette.Common.Dto;
using Moniquette.Common.Models;

namespace Moniquette.Client.Api;

public class BaseHttpApi(
    IOptions<ClientConfig> configOptions,
    IHttpClientFactory httpClientFactory) 
    : IBaseApi
{
    private ClientConfig Config { get; } = configOptions.Value;
    
    private HttpClient HttpClient { get; } = httpClientFactory.CreateClient(nameof(BaseHttpApi));
    
    private string? BearerToken { get; set; }
    
    public async Task<IApiResult> RegisterAsync(
        RegistrationRequestDto requestDto, 
        CancellationToken cancellationToken = default)
    {
        var uriBuilder = BuildCustomUri(Config.BaseHttpAddress, Routes.Registration);
        var httpResponse = await HttpClient.PostAsJsonAsync(uriBuilder.Uri, requestDto, cancellationToken);
        var result = await GetIApiResult<RegistrationResponseDto>(httpResponse);
        if (result is ApiOk<RegistrationResponseDto> { Value: not null } ok)
        {
            BearerToken = ok.Value.Token;
        }

        return result;
    }
    
    // public async Task<IApiResult> SendPing(Report report) // нужно пинговать чаще

    public async Task<IApiResult> SendReportAsync(
        Report request, 
        CancellationToken cancellationToken = default)
    {
        var uriBuilder = BuildCustomUri(Config.BaseHttpAddress, Routes.SendReport);
        if (!string.IsNullOrWhiteSpace(BearerToken))
        {
            HttpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", BearerToken);
        }

        var httpResponse = await HttpClient.PostAsJsonAsync(uriBuilder.Uri, request, cancellationToken);
        return await GetIApiResult(httpResponse);
    }

    private static async Task<IApiResult> GetIApiResult<TResponseData>(
        HttpResponseMessage httpResponse) 
        where TResponseData : class
    {
        if (!httpResponse.IsSuccessStatusCode)
            return Results.Error(new HttpError
            {
                ErrorCode = httpResponse.StatusCode,
                Message = httpResponse.ReasonPhrase
            });
        
        var response = await httpResponse.Content.ReadFromJsonAsync<TResponseData>();
        return Results.Ok(response!);
    }
    
    private static Task<IApiResult> GetIApiResult(HttpResponseMessage httpResponse)
    {
        var result = 
            httpResponse.IsSuccessStatusCode 
                ? Results.Ok() 
                : Results.Error(new HttpError {
                    ErrorCode = httpResponse.StatusCode,
                    Message = httpResponse.ReasonPhrase
                });
        return Task.FromResult(result);
    }

    private static UriBuilder BuildCustomUri(params string[] args) 
        => new(string.Join('/', args.Select(s => s.Trim('/'))));
}
