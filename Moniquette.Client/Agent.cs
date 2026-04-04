using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moniquette.Client.Config;
using Moniquette.Client.Pipeline;
using Moniquette.Common.Api;
using Moniquette.Common.Api.Result;
using Moniquette.Common.Dto;
using Moniquette.Common.Exceptions;
using Moniquette.Common.Models;

namespace Moniquette.Client;

public class Agent(IServiceProvider serviceProvider)
{
    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var config = GetConfig();
        var baseApi = GetBaseApi();

        var response = await RegisterAsync(baseApi, config.UserInfo);
        if (response is null)
        {
            return;
        }

        Logger.LogInformation("Successful registration! your credentials: {credentials}",
            JsonSerializer.Serialize(response));

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var report = await CreateReportAsync(response.Token, cancellationToken);
                Logger.LogInformation("Sending report...\n{report}", JsonSerializer.Serialize(report));
                await baseApi.SendReportAsync(report, cancellationToken);
            }
            catch (TaskCanceledException)
            {
                return;
            }
            catch (WrongPrivilegesException e)
            {
                Logger.LogError(e, "WrongPrivilegesException occurred while sending report.");
                return;
            }
            catch (Exception e)
            {
                Logger.LogError(e, "An exception occurred while sending report.");
            }
            finally
            {
                await Task
                    .Delay(config.ReportDelayMs, cancellationToken)
                    .ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);
            }
        }
    }
    
    private ILogger<Agent> Logger { get; } = serviceProvider.GetService<ILogger<Agent>>()!;

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

    /// <summary>
    /// Registers new user in system 
    /// </summary>
    /// <param name="baseApi">Moniquette Server API realization</param>
    /// <param name="requestDto">Registration request</param>
    /// <returns>User authorization token wrapped in RegistrationResponseDto</returns>
    private async Task<RegistrationResponseDto?> RegisterAsync(IBaseApi baseApi, RegistrationRequestDto requestDto)
    {
        var response = await baseApi.RegisterAsync(requestDto);
        if (response.Success)
        {
            return (response as ApiOk<RegistrationResponseDto>)?.Value;
        }

        Logger.LogError("Registration response is invalid: {response}", (response as ApiError));
        return null;
    }

    private async Task<Report> CreateReportAsync(string token, CancellationToken ct = default)
    {
        var sessionId = Guid.Parse(token);
        var scopeFactory = serviceProvider.GetService<IServiceScopeFactory>()
                           ?? throw new Exception("An exception occurred while creating a scope");
        using var scope = scopeFactory.CreateScope();
        var reportPipeline = new ReportPipeline(scope.ServiceProvider);
        var report = await reportPipeline.RunAsync(sessionId, ct);
        return report;
    }
}