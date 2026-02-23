using Moniquette.Common.Models;
using LinuxUtils = Moniquette.Common.Utils.LinuxUtils;

namespace Moniquette.Client.Services;

public class WmctrlService
{
    public List<ActiveView> GetX11Views()
    {
        var windowsString = GetWindowInfo();
        if (string.IsNullOrWhiteSpace(windowsString))
            throw new Exception("Wmctrl service returned an empty string");

        return windowsString
            .Trim()
            .Split('\n')
            .Select(fieldsString =>
            {
                var fields = fieldsString.Split(';');
                if (fields.Length != 4) // Should contain: ID, PID, ClassName, Title
                    throw new Exception("Wmctrl returned an unexpected number of fields");

                return new ActiveView
                {
                    Id = Convert.ToInt64(fields[0], 16),
                    Pid = int.Parse(fields[1]),
                    Class = fields[2],
                    Title = fields[3],
                };
            }).ToList();
    }

    private string GetWindowInfo()
    {
        const string awkCommand = "{print $1 \";\" $3 \";\" $4 \";\" substr($0, index($0,$6))}";
        const string fullCommand = $"wmctrl -lpx | awk '{awkCommand}'";
        var processResult = LinuxUtils.RunShellScript(fullCommand);
        if (!processResult)
        {
            throw new Exception($"Exception occured while running Wmctrl service{Environment.NewLine}" +
                                processResult);
        }

        return processResult.Value;
    }
}