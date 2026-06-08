using Moniquette.Client.Services.Abstractions;
using Moniquette.Common.Models;
using Microsoft.Extensions.Logging;

namespace Moniquette.Client.Pipeline.Fillers;

public class DockerFiller(
    IDockerService service,
    ILogger<DockerFiller> logger) : IReportFiller
{
    public Task<Report> Fill(Report request, CancellationToken cancellationToken)
    {
        request.IsDockerEnabled = service.CheckDockerIsRunning();
        if (request.IsDockerEnabled)
        {
            try
            {
                var output = service.GetRunningDockerContainers();
                request.DockerContainers = output;
            }
            catch (Exception e)
            {
                logger.LogWarning(e, "Docker is running, but running containers could not be collected.");
                request.DockerContainers = [];
            }
        }
        return Task.FromResult(request);
    }
}
