using Grpc.Net.Client;
using Hardware.Info;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moniquette.Client.Api;
using Moniquette.Client.Config;
using Moniquette.Client.Pipeline.Fillers;
using Moniquette.Client.Services;
using Moniquette.Client.Services.Abstractions;
using Moniquette.Common.Api;
using Moniquette.Common.Utils;
using Tmds.DBus.Protocol;

namespace Moniquette.Client.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLoggingReady(this IServiceCollection services)
        => services
            .AddLogging(builder => builder
                .AddFilter("Microsoft", LogLevel.Warning)
                .AddFilter("System", LogLevel.Warning)
                .AddFilter("LoggingConsoleApp.Program", LogLevel.Debug)
                .AddConsole());

    public static IServiceCollection AddServices(this IServiceCollection services)
        => services
            .AddSingleton<GnomeWindowsExtensionService>(_ =>
            {
                var connection = new Connection(Address.Session!);
                connection.ConnectAsync().ConfigureAwait(true);
                return new GnomeWindowsExtensionService(connection);
            })
            .AddScoped<WmctrlService>()
            .AddScoped<IUsbDevicesService, UsbDevicesService>()
            .AddBluetoothServices();
    
    private static IServiceCollection AddBluetoothServices(this IServiceCollection services)
    {
        var service = services.BuildServiceProvider().GetService<OperatingSystemService>();
        if (service is null)
        {
            throw new NullReferenceException(nameof(service));
        }
        switch (service.GetOperatingSystem())
        {
            case Literals.Linux:
                services.AddScoped<IBluetoothDevicesService, LinuxBluetoothDevicesService>();
                break;
            case Literals.Windows:
                // add windows service
                break;
            default:
                // add logging and throw error
                throw new NotImplementedException();
        }

        return services;
    }

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
            .AddSingleton<IHardwareInfo, HardwareInfo>(_ =>
            {
                var hardwareInfo = new HardwareInfo();
                hardwareInfo.RefreshAll();
                return hardwareInfo;
            })
            .AddTransient<OperatingSystemService>(); // HardwareInfo must be enabled

    public static IServiceCollection AddFillers(
        this IServiceCollection services)
        => services
            .AddTransient<IReportFiller, HardwareFiller>()
            .AddTransient<IReportFiller, ActiveViewFiller>()
            .AddTransient<IReportFiller, NetworkFiller>()
            .AddTransient<IReportFiller, WindowsRegistryFiller>()
            .AddTransient<IReportFiller, DockerFiller>();
    // .AddTransient<IReportFiller, BluetoothDevicesFiller>();

    public static IServiceCollection AddGrpcChannel(this IServiceCollection services)
        => services
            .AddSingleton<GrpcChannel>(provider =>
            {
                var config = provider.GetRequiredService<IOptions<ClientConfig>>().Value;

                var logger = provider.GetRequiredService<ILogger<GrpcChannel>>();
                logger.LogInformation("Initializing gRPC channel for address {address}...", config.BaseGrpcAddress);
                return GrpcChannel.ForAddress(config.BaseGrpcAddress);
            });

    public static IServiceCollection AddApi(this IServiceCollection services)
        => services
            .AddScoped<IBaseApi, BaseHttpApi>()
            .AddScoped<IBaseApi, BaseGrpcApi>();
}