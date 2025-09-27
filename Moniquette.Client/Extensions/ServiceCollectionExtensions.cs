using Hardware.Info;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moniquette.Client.Api;
using Moniquette.Client.Config;
using Moniquette.Client.Pipeline.Fillers;
using Moniquette.Common.Api;

namespace Moniquette.Client.Extensions;

public static class ServiceCollectionExtensions
{
    private const string ConfigurationPath = "";

    public static IServiceCollection AddLoggingReady(this IServiceCollection services)
        => services
            .AddLogging(builder => builder
                .AddFilter("Microsoft", LogLevel.Warning)
                .AddFilter("System", LogLevel.Warning)
                .AddFilter("LoggingConsoleApp.Program", LogLevel.Debug)
                .AddConsole());
    
    public static IServiceCollection AddConfiguration(this IServiceCollection services)
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", false, true)
            .AddJsonFile($"appsettings.{env}s.json", true, true)
            .Build();
        return services
            .Configure<ClientConfig>(configuration.GetSection("Application"));
    }

    public static IServiceCollection AddHardwareInfo(this IServiceCollection services)
        => services
            .AddScoped<IHardwareInfo, HardwareInfo>(_ =>
            {
                var hardwareInfo = new HardwareInfo();
                hardwareInfo.RefreshAll();
                return hardwareInfo;
            });
    
    public static IServiceCollection AddFillers(
        this IServiceCollection services)
        => services
            .AddTransient<IReportFiller, HardwareFiller>()
            .AddTransient<IReportFiller, ActiveViewFiller>()
            .AddTransient<IReportFiller, NetworkReportFiller>()
            .AddTransient<IReportFiller, WindowsRegistryFiller>();

    public static IServiceCollection AddApi(this IServiceCollection services)
        => services
            .AddScoped<IBaseApi, BaseApi>();
}