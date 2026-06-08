using System.Text.Json;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MessagePack;
using Moniquette.Common.Models;
using Moniquette.Server.Analysis;
using Moniquette.Server.Auth;
using Moniquette.Server.Hubs;
using Moniquette.Server.Repositories;
using gRPC = Moniquette.Common.gRPC;

namespace Moniquette.Server.Services;

public class ReportService(
    ILogger<ReportService> logger,
    IJwtTokenService jwtTokenService,
    ISessionRepository sessionRepository,
    IReportRepository reportRepository,
    IReportAnalysisQueue reportAnalysisQueue,
    IMonitoringNotifier monitoringNotifier
    ) : gRPC.ReportService.ReportServiceBase
{
    public override async Task<gRPC.RegistrationResponse> Register(gRPC.RegistrationRequest request, ServerCallContext context)
    {
        ValidateRegistrationRequest(request);

        var session = new Session
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName.Trim(),
            MiddleName = request.MiddleName.Trim(),
            LastName = request.LastName.Trim(),
            HardwareInfo = CreateEmptyHardwareInfo()
        };

        try
        {
            await sessionRepository.SaveAsync(session, context.CancellationToken);
            var token = jwtTokenService.CreateToken(session);
            return new gRPC.RegistrationResponse
            {
                Token = token,
                SessionId = session.Id.ToString()
            };
        }
        catch (RpcException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to register session for {LastName} {FirstName}",
                request.LastName,
                request.FirstName);
            throw new RpcException(new Status(StatusCode.Internal, "Failed to register session."));
        }
    }

    public override async Task<Empty> SendReport(gRPC.ReportMessage request, ServerCallContext context)
    {
        try
        {
            var authorizedSessionId = await AuthorizeAsync(context);
            var messageBytes = new byte[request.Data.Length];
            request.Data.CopyTo(messageBytes, 0);
            var stream = new MemoryStream(messageBytes);
            var report = await MessagePackSerializer
                .DeserializeAsync<Report>(stream, SerializerOptions, context.CancellationToken);

            await ValidateReportAsync(report, authorizedSessionId, context.CancellationToken);
            await reportRepository.SaveAsync(report, context.CancellationToken);
            await monitoringNotifier.NotifyReportReceivedAsync(report, context.CancellationToken);
            await reportAnalysisQueue.EnqueueAsync(report);

            logger.LogInformation("Report {ReportId} for session {SessionId} saved and queued for analysis.",
                report.Id,
                report.SessionId);
            logger.LogDebug("Report payload: {Report}", JsonSerializer.Serialize(report));
            return new Empty();
        }
        catch (RpcException)
        {
            throw;
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Invalid report payload.");
            throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to process report.");
            throw new RpcException(new Status(StatusCode.Internal, "Failed to process report."));
        }
    }
    
    private static MessagePackSerializerOptions SerializerOptions { get; } =
        MessagePack.Resolvers.ContractlessStandardResolver.Options
            .WithCompression(MessagePackCompression.Lz4BlockArray);

    private async Task<Guid> AuthorizeAsync(ServerCallContext context)
    {
        var authorization = context.RequestHeaders.GetValue("authorization");
        if (string.IsNullOrWhiteSpace(authorization)
            || !authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            throw new RpcException(new Status(StatusCode.Unauthenticated, "Missing bearer token."));
        }

        var token = authorization["Bearer ".Length..].Trim();
        var validation = jwtTokenService.ValidateToken(token);
        if (!validation.IsValid)
        {
            logger.LogWarning("JWT validation failed: {Error}", validation.Error);
            throw new RpcException(new Status(StatusCode.Unauthenticated, "Invalid bearer token."));
        }

        var sessionExists = await sessionRepository.ExistsAsync(validation.SessionId, context.CancellationToken);
        if (!sessionExists)
        {
            throw new RpcException(new Status(StatusCode.Unauthenticated, "Session from token was not found."));
        }

        return validation.SessionId;
    }

    private async Task ValidateReportAsync(
        Report report,
        Guid authorizedSessionId,
        CancellationToken cancellationToken)
    {
        if (report.Id == Guid.Empty)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Report Id is required."));
        }

        if (report.SessionId == Guid.Empty)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Report SessionId is required."));
        }

        if (report.SessionId != authorizedSessionId)
        {
            throw new RpcException(new Status(StatusCode.PermissionDenied,
                "Report SessionId does not match authorized session."));
        }

        if (report.HardwareInfo is null)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Report HardwareInfo is required."));
        }

        var sessionExists = await sessionRepository.ExistsAsync(report.SessionId, cancellationToken);
        if (!sessionExists)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "Session was not found."));
        }
    }

    private static void ValidateRegistrationRequest(gRPC.RegistrationRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FirstName)
            || string.IsNullOrWhiteSpace(request.LastName)
            || string.IsNullOrWhiteSpace(request.SerialNumber))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument,
                "FirstName, LastName and SerialNumber are required."));
        }
    }

    private static HardwareBriefInfo CreateEmptyHardwareInfo()
        => new()
        {
            OperatingSystem = "Unknown",
            AvailableRam = 0,
            MouseList = [],
            SoundDeviceList = [],
            UsbDevices = [],
            BluetoothDevices = []
        };
}
