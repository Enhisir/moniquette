using System.Text;
using System.Text.RegularExpressions;
using Moniquette.Client.Services.Abstractions;
using Moniquette.Common.Exceptions;
using Moniquette.Common.Models.Hardware;
using Moniquette.Common.Utils;

namespace Moniquette.Client.Services.Linux;

public partial class LinuxBluetoothDevicesService : IBluetoothDevicesService
{
    private const string ShellCommand =
        "bluetoothctl devices | cut -f2 -d' ' | while read uuid; do bluetoothctl info $uuid; done";

    public List<BluetoothDevice> GetAttached()
    {
        var processResult = LinuxUtils.RunShellScript(ShellCommand);
        if (!processResult)
        {
            // Log Message
            return [];
        }

        return SplitDeviceBlocks(processResult.Value)
            .Select(ParseDeviceInfo)
            .WhereNotNull()
            .ToList();
    }

    private static IEnumerable<string> SplitDeviceBlocks(string text)
    {
        var result = new List<string>();
        var reader = new StringReader(text);
        var sb = new StringBuilder();

        while (reader.ReadLine() is { } line)
        {
            line = line.Trim();
            if (line.StartsWith("Device "))
            {
                if (sb.Length > 0)
                {
                    result.Add(sb.ToString());
                    sb.Clear();
                }
            }

            if (sb.Length > 0)
                sb.AppendLine();

            sb.Append(line);
        }

        if (sb.Length > 0)
            result.Add(sb.ToString());

        return result;
    }

    private static BluetoothDevice? ParseDeviceInfo(string text)
    {
        BluetoothDevice? device = null;

        foreach (var line in text.Split(Environment.NewLine).Select(x => x.Trim()))
        {
            var deviceMatch = DeviceRegex().Match(line);
            if (deviceMatch.Success)
            {
                device = new BluetoothDevice
                {
                    Address = deviceMatch.Groups[1].Value
                };
                continue;
            }

            if (device == null)
                continue;

            var nameMatch = DeviceNameRegex().Match(line);
            if (nameMatch.Success)
            {
                device.Name = nameMatch.Groups[1].Value.Trim();
                continue;
            }

            var classMatch = ClassRegex().Match(line);
            if (classMatch.Success)
            {
                device.Class = classMatch.Groups[1].Value.Trim();
                continue;
            }

            var connectedMatch = ConnectedRegex().Match(line);
            if (connectedMatch.Success)
            {
                var connected = string.Equals(
                    connectedMatch.Groups[1].Value.Trim(),
                    "yes",
                    StringComparison.OrdinalIgnoreCase);

                if (!connected)
                    return null;
            }

            var uuidMatch = UuidRegex().Match(line);
            if (uuidMatch.Success)
            {
                device.Profiles.Add(new BluetoothProfile
                {
                    Name = uuidMatch.Groups[1].Value.Trim(),
                    Uuid = uuidMatch.Groups[2].Value.Trim()
                });
            }
        }

        return device;
    }

    // private static IEnumerable<string> SplitLines(string text) =>
    //     text.Replace("\r\n", "\n").Split('\n', StringSplitOptions.RemoveEmptyEntries);

    [GeneratedRegex(@"^Device\s+([0-9A-Fa-f:]+)")]
    private static partial Regex DeviceRegex();

    [GeneratedRegex(@"^\s*Name:\s*(.+)$")]
    private static partial Regex DeviceNameRegex();

    [GeneratedRegex(@"^\s*Class:\s*(.+)$")]
    private static partial Regex ClassRegex();

    [GeneratedRegex(@"^\s*Connected:\s*(.+)$")]
    private static partial Regex ConnectedRegex();

    [GeneratedRegex(@"^\s*UUID:\s*(.+?)\s+\(([0-9A-Fa-f-]+)\)$")]
    private static partial Regex UuidRegex();
}