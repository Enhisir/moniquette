using Microsoft.AspNetCore.SignalR;

namespace Moniquette.Server.Hubs;

public class MonitoringHub : Hub
{
    public Task JoinSessionGroup(string sessionId)
        => Groups.AddToGroupAsync(Context.ConnectionId, MonitoringHubGroups.Session(sessionId));

    public Task LeaveSessionGroup(string sessionId)
        => Groups.RemoveFromGroupAsync(Context.ConnectionId, MonitoringHubGroups.Session(sessionId));
}

public static class MonitoringHubGroups
{
    public static string Session(Guid sessionId)
        => Session(sessionId.ToString());

    public static string Session(string sessionId)
        => $"session:{sessionId}";
}
