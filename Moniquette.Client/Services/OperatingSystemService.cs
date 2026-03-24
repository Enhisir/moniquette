using Hardware.Info;
using Moniquette.Common.Utils;

namespace Moniquette.Client.Services;

public class OperatingSystemService(IHardwareInfo hardwareInfo)
{
    public string GetOperatingSystem()
    {
        var fullname = hardwareInfo.OperatingSystem.Name.ToLower();
        if (fullname.Contains("windows")) return Literals.Windows;
        if (fullname.Contains("linux")) return Literals.Linux;
        if (fullname.Contains("macos")) return Literals.MacOS;
        return fullname;
    }
}