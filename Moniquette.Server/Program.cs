using Moniquette.Server.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddGrpc();

var app = builder.Build();

// var sharedSecret = Environment.GetEnvironmentVariable("SHARE_SECRET");
app.MapGrpcService<ReportService>();

app.Run();
