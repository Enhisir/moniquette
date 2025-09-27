using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using Moniquette.Client.Config;
using Moniquette.Common.Api;
using Moniquette.Common.Api.Result;
using Moniquette.Common.Dto;
using Moniquette.Common.Models;

namespace Moniquette.Client.Api;

public class BaseApi(
    IOptions<ClientConfig> configOptions,
    IHttpClientFactory httpClientFactory) 
    : IBaseApi
{
    private ClientConfig Config { get; } = configOptions.Value;
    
    private HttpClient HttpClient { get; } = httpClientFactory.CreateClient(nameof(BaseApi));
    
    public async Task<IApiResult> Register(RegistrationRequest request)
    {
        var uriBuilder = BuildCustomUri(Config.BaseAddress, Routes.Registration);
        var httpResponse = await HttpClient.PostAsJsonAsync(uriBuilder.Uri, request);
        return await GetIApiResult<RegistrationResponse>(httpResponse);
    }
    
    // public async Task<IApiResult> SendPing(Report report) // нужно пинговать чаще

    public async Task<IApiResult> SendReport(Report request)
    {
        var uriBuilder = BuildCustomUri(Config.BaseAddress, Routes.SendReport);
        var httpResponse = await HttpClient.PostAsJsonAsync(uriBuilder.Uri, request);
        return await GetIApiResult(httpResponse);
    }

    private static async Task<IApiResult> GetIApiResult<TResponseData>(HttpResponseMessage httpResponse) where TResponseData : class
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