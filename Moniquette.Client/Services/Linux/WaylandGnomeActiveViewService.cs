using System.Text.Json;
using Moniquette.Common.Models;
using Tmds.DBus.Protocol;

namespace Moniquette.Client.Services.Linux;

public class WaylandGnomeActiveViewService(Connection dbusConnection)
{
    private const string Peer = "org.gnome.Shell";
    private const string Interface = "org.gnome.Shell.Extensions.WindowsExt";
    private const string Path = "/org/gnome/Shell/Extensions/WindowsExt";
    
    public async Task<List<ActiveView>> ListActiveViews() 
        => await dbusConnection.CallMethodAsync(
            GetListMessage(),
            (message, _) =>
            {
                var jsonString = message.GetBodyReader().ReadString();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<List<ActiveView>>(jsonString, options) ?? [];
            });

    private MessageBuffer GetListMessage()
    {
        using var writer = dbusConnection.GetMessageWriter();
        writer.WriteMethodCallHeader(
            destination: Peer,
            path: Path,
            @interface: Interface,
            signature: "",
            member: "List");
        return writer.CreateMessage();
    }
}