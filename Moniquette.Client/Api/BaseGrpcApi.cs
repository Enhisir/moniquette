using Google.Protobuf;
using Grpc.Core;
using Grpc.Net.Client;
using MessagePack;
using Moniquette.Common.Api;
using Moniquette.Common.Api.Result;
using Moniquette.Common.Dto;
using Moniquette.Common.Models;
using gRPC = Moniquette.Common.gRPC;

namespace Moniquette.Client.Api;

public class BaseGrpcApi(GrpcChannel grpcChannel) : IBaseApi
{
    private gRPC.ReportService.ReportServiceClient Client { get; } = new(grpcChannel);
    private string? BearerToken { get; set; }
    
    // private AsyncClientStreamingCall<gRPC.ReportMessage, Empty>? SendReportCall { get; set; }

    private MessagePackSerializerOptions SerializerOptions { get; } =
        MessagePack.Resolvers.ContractlessStandardResolver.Options
            .WithCompression(MessagePackCompression.Lz4BlockArray);

    public async Task<IApiResult> RegisterAsync(
        RegistrationRequestDto requestDto,
        CancellationToken ct = default)
    {
        try
        {
            var response = await Client.RegisterAsync(new gRPC.RegistrationRequest
            {
                FirstName = requestDto.FirstName,
                MiddleName = requestDto.MiddleName,
                LastName = requestDto.LastName,
                SerialNumber = requestDto.SerialNumber
            }, cancellationToken: ct);

            if (response is null)
            {
                return Results.Error("Registration failed: no response.");
            }
            
            // SendReportCall = Client.SendReportStream(cancellationToken: ct);
            BearerToken = response.Token;
            return Results.Ok(new RegistrationResponseDto
            {
                Token = response.Token,
                SessionId = response.SessionId
            });
        }
        catch (Exception e)
        {
            return Results.Error(e.Message);
        }
    }

    public async Task<IApiResult> SendReportAsync(
        Report report,
        CancellationToken ct = default)
    {
        try
        {
            /* 
            if (SendReportCall is null)
                return Results.Error("Cannot send report. Registration failed or client is not registered.");
            */
            
            var byteStream = new MemoryStream();
            await MessagePackSerializer.SerializeAsync(byteStream, report, SerializerOptions, ct);
            var bytes = byteStream.ToArray();
            var data = ByteString.CopyFrom(bytes); // ends null
            var reportMessage = new gRPC.ReportMessage { Data = data };
            await Client.SendReportAsync(reportMessage, CreateAuthMetadata(), cancellationToken: ct);
            return Results.Ok();
        }
        catch (Exception e)
        {
            return Results.Error(e.Message);
        }
    }

    private Metadata CreateAuthMetadata()
        => string.IsNullOrWhiteSpace(BearerToken)
            ? []
            : [new Metadata.Entry("authorization", $"Bearer {BearerToken}")];
}
