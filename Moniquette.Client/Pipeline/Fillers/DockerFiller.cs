using Moniquette.Client.Services.Abstractions;
using Moniquette.Common.Models;

namespace Moniquette.Client.Pipeline.Fillers;

public class DockerFiller(IDockerService service) : IReportFiller
{
    public Task<Report> Fill(Report request, CancellationToken cancellationToken)
    {
        if (service.CheckDockerIsRunning())
        {
            try
            {
                var output = service.GetRunningDockerContainers();
                request.DockerContainers = output;
            }
            catch (Exception e)
            {
                // TODO: log exception
                request.DockerContainers = null;
            }
        }
        return Task.FromResult(request);
    }
}