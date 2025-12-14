using System.Text.Json;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MessagePack;
using Moniquette.Common.Models;
using gRPC = Moniquette.Common.gRPC;

namespace Moniquette.Server.Services;

public class ReportService(
    ILogger<ReportService> logger
    ) : gRPC.ReportService.ReportServiceBase
{
    public override async Task<gRPC.RegistrationResponse> Register(gRPC.RegistrationRequest request, ServerCallContext context)
    {
        var token = Guid.NewGuid().ToString();
        return await Task.FromResult(new gRPC.RegistrationResponse()
        {
            Token = token
        });
    }

    public override async Task<Empty> SendReport(gRPC.ReportMessage request, ServerCallContext context)
    {
        try
        {
            var messageBytes = new byte[request.Data.Length];
            request.Data.CopyTo(messageBytes, 0);
            var stream = new MemoryStream(messageBytes);
            var report = await MessagePackSerializer
                .DeserializeAsync<Report>(stream, SerializerOptions, context.CancellationToken);

            Console.WriteLine(JsonSerializer.Serialize(report));
            return new Empty();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
        }
        
        return new Empty();
    }

    /*
    public override async Task<Empty> SendReportStream(IAsyncStreamReader<gRPC.ReportMessage> requestStream, ServerCallContext context)
    {
        await foreach(var request in requestStream.ReadAllAsync())
        {
            await ProcessMessageAsync(request, context.CancellationToken);
        }
        
        return new Empty();
    }

    private async Task ProcessMessageAsync(gRPC.ReportMessage message, CancellationToken ct)
    {
        var messageBytes = new byte[message.Data.Length];
        message.Data.CopyTo(messageBytes, 0);
        var stream = new MemoryStream(messageBytes);
        var report = await MessagePackSerializer
            .DeserializeAsync<Report>(stream, SerializerOptions, ct);
        
        Console.WriteLine(JsonSerializer.Serialize(report));
    }
    */
    
    private static MessagePackSerializerOptions SerializerOptions { get; } =
        MessagePack.Resolvers.ContractlessStandardResolver.Options
            .WithCompression(MessagePackCompression.Lz4BlockArray);
}