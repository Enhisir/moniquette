using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moniquette.Client.Api;
using Moniquette.Client.Config;
using Moniquette.Client.Pipeline;
using Moniquette.Common.Api;
using Moniquette.Common.Dto;
using Moniquette.Common.Models;

namespace Moniquette.Client;

public class Agent(IServiceProvider serviceProvider)
{
    public const int DelayAmount = 150_000;
    
    private ILogger<Agent> Logger { get; } = serviceProvider.GetService<ILogger<Agent>>()!;
    
    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var config = GetConfig();
        var baseApi = GetBaseApi();

        var response = await RegisterAsync(baseApi, config.UserInfo);
        
        Logger.LogInformation("Successful registration! your credentials: {credentials}", JsonSerializer.Serialize(response));
        
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var report = await CreateReportAsync(cancellationToken);
                Logger.LogInformation("Sending report...\n{report}", JsonSerializer.Serialize(report));
                await baseApi.SendReport(report);
            }
            catch (TaskCanceledException)
            {
                break;
            }
            catch (Exception e)
            {
                Logger.LogError(e, "An exception occurred while sending report.");
            }
            finally
            {
                await Task.Delay(DelayAmount, cancellationToken);
            }
        }
    }

    private ClientConfig GetConfig()
    {
        var configOptions = serviceProvider.GetService<IOptions<ClientConfig>>();
        if (configOptions is null) 
            throw new InvalidOperationException("Missing configuration options.");
        
        return configOptions.Value;
    }
    
    private IBaseApi GetBaseApi()
        => serviceProvider.GetService<IBaseApi>() 
           ?? throw new InvalidOperationException(
               $"Missing initialization of {nameof(IBaseApi)} in ServiceProvider.");

    private async Task<RegistrationResponse> RegisterAsync(IBaseApi baseApi, RegistrationRequest request)
    {
        return new RegistrationResponse() { Token = "new default token" };
        /*
        var response = await baseApi.Register(request);
        if (response.Success)
            return (response as ApiOk<RegistrationResponse>)?.Value
                   ?? throw new InvalidCastException("Registration response is invalid.");

        var error = response as ApiError;
        throw new InvalidOperationException(
            "Registration ended up with failure. " +
            $"Error code: {error!.Error.ErrorCode}" +
            $"Error message: {error.Error.Message}");
        */
    }
    
    private async Task<Report> CreateReportAsync(CancellationToken ct = default)
    {
        var scopeFactory = serviceProvider.GetService<IServiceScopeFactory>() 
                           ?? throw new Exception(); // fill in the info about exception
        using var scope = scopeFactory.CreateScope();
        var reportPipeline = new ReportPipeline(scope.ServiceProvider);
        var report = await reportPipeline.RunAsync(ct);
        return report;
    }
}