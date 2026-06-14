using Elastic.Clients.Elasticsearch;
using Moniquette.Elastic.Infrastructure;
using Moniquette.Elastic.Services;
using Moniquette.Server.Analysis;
using Moniquette.Server.Auth;
using Moniquette.Server.Hubs;
using Moniquette.Server.Repositories;
using Moniquette.Server.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddGrpc();
builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddCors(options =>
{
    options.AddPolicy("MoniquetteAdminDevelopment", policy =>
        policy
            .WithOrigins("http://localhost:5173", "http://127.0.0.1:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.Configure<NetworkPolicyOptions>(builder.Configuration.GetSection("NetworkPolicy"));
builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>();
builder.Services.AddSingleton(_ =>
{
    var elasticUrl = builder.Configuration["Urls:ElasticSearch"];
    if (string.IsNullOrWhiteSpace(elasticUrl))
    {
        throw new InvalidOperationException("Missing Elasticsearch URL configuration.");
    }

    var settings = new ElasticsearchClientSettings(new Uri(elasticUrl))
        .DefaultIndex("sessions")
        .DisableDirectStreaming();
    return new ElasticsearchClient(settings);
});
builder.Services.AddSingleton<ElasticSetupService>();
builder.Services.AddSingleton<IProcessBandService, ProcessBandService>();
builder.Services.AddScoped<ISessionRepository, ElasticSessionRepository>();
builder.Services.AddScoped<IReportRepository, ElasticReportRepository>();
builder.Services.AddScoped<IThreatRepository, ElasticThreatRepository>();
builder.Services.AddScoped<ISuspiciousProcessRepository, ElasticSuspiciousProcessRepository>();
builder.Services.AddScoped<ISuspiciousDockerImageRepository, ElasticSuspiciousDockerImageRepository>();
builder.Services.AddSingleton<IMonitoringNotifier, SignalRMonitoringNotifier>();
builder.Services.AddSingleton<IReportAnalysisQueue, ReportAnalysisQueue>();
builder.Services.AddHostedService<ReportAnalysisWorker>();
builder.Services.AddScoped<IReportAnalysisPipeline, ReportAnalysisPipeline>();
builder.Services.AddScoped<IReportAnalyzer, VirtualizationAnalyzer>();
builder.Services.AddScoped<IReportAnalyzer, UsbDeviceAnalyzer>();
builder.Services.AddScoped<IReportAnalyzer, BluetoothDeviceAnalyzer>();
builder.Services.AddScoped<IReportAnalyzer, ProcessAnalyzer>();
builder.Services.AddScoped<IReportAnalyzer, DockerImageAnalyzer>();
builder.Services.AddScoped<IReportAnalyzer, NetworkAnalyzer>();

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

await app.Services.GetRequiredService<ElasticSetupService>()
    .SetupAsync(app.Lifetime.ApplicationStopping);


app.MapOpenApi();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/openapi/v1.json", "v1");
});

app.UseCors("MoniquetteAdminDevelopment");
app.MapControllers();
app.MapHub<MonitoringHub>("/monitoring-hub");
app.MapGrpcService<ReportService>();

app.Run();
