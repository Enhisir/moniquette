using Moniquette.Common.Enums;
using Moniquette.Common.Models;
using Moniquette.Common.Models.Hardware;

namespace Moniquette.Server.Analysis;

public class BluetoothDeviceAnalyzer : AnalyzerBase
{
    private const uint MajorServiceNetworking = 0x020000;
    private const uint MajorServiceObjectTransfer = 0x100000;
    private const uint MajorServiceAudio = 0x200000;
    private const uint MajorServiceTelephony = 0x400000;

    private const uint MajorDeviceComputer = 0x01;
    private const uint MajorDevicePhone = 0x02;
    private const uint MajorDeviceLanNetwork = 0x03;
    private const uint MajorDeviceAudioVideo = 0x04;
    private const uint MajorDeviceWearable = 0x07;
    private const uint MajorDeviceHealth = 0x09;

    private static readonly IReadOnlyCollection<BluetoothRule> Rules =
    [
        DetectPhoneClass,
        DetectComputerOrNetworkClass,
        DetectWearableOrHealthClass,
        DetectRiskyServiceClass,
        DetectRiskyProfiles
    ];

    public override Task<IReadOnlyCollection<Threat>> AnalyzeAsync(
        Report report,
        CancellationToken cancellationToken)
    {
        var threats = report.HardwareInfo.BluetoothDevices
            .SelectMany(device => Rules
                .Select(rule => rule(device))
                .Where(reason => !string.IsNullOrWhiteSpace(reason))
                .Select(reason => CreateThreat(
                    report,
                    ThreatType.Warning,
                    $"Подозрительное Bluetooth-устройство: {FormatDevice(device)}. {reason}")))
            .ToList();

        return Task.FromResult<IReadOnlyCollection<Threat>>(threats);
    }

    private static string? DetectPhoneClass(BluetoothDevice device)
    {
        if (GetMajorDeviceClass(device) == MajorDevicePhone)
        {
            return "Class of Device указывает Major Device Class = Phone.";
        }

        return ContainsAny(device.Name, "phone", "android", "iphone", "смартфон", "телефон")
            ? "Название устройства содержит признаки телефона или смартфона."
            : null;
    }

    private static string? DetectComputerOrNetworkClass(BluetoothDevice device)
    {
        var major = GetMajorDeviceClass(device);
        return major switch
        {
            MajorDeviceComputer => "Class of Device указывает Major Device Class = Computer.",
            MajorDeviceLanNetwork => "Class of Device указывает Major Device Class = LAN/Network Access Point.",
            _ => null
        };
    }

    private static string? DetectWearableOrHealthClass(BluetoothDevice device)
    {
        if (IsProbablyAudio(device))
        {
            return null;
        }

        var major = GetMajorDeviceClass(device);
        return major switch
        {
            MajorDeviceWearable => "Class of Device указывает Major Device Class = Wearable.",
            MajorDeviceHealth => "Class of Device указывает Major Device Class = Health.",
            _ => ContainsAny(device.Name, "watch", "часы", "band", "браслет")
                ? "Название устройства содержит признаки wearable/smart-устройства."
                : null
        };
    }

    private static string? DetectRiskyServiceClass(BluetoothDevice device)
    {
        if (device.ClassOfDevice == 0)
        {
            return null;
        }

        var riskyServices = new List<string>();
        if (HasService(device, MajorServiceNetworking))
        {
            riskyServices.Add("Networking");
        }

        if (HasService(device, MajorServiceObjectTransfer))
        {
            riskyServices.Add("Object Transfer");
        }

        if (HasService(device, MajorServiceTelephony))
        {
            riskyServices.Add("Telephony");
        }

        if (riskyServices.Count == 0)
        {
            return null;
        }

        if (IsProbablyAudio(device)
            && riskyServices.All(service => service == "Telephony")
            && HasService(device, MajorServiceAudio))
        {
            return null;
        }

        return $"Class of Device содержит service class: {string.Join(", ", riskyServices)}.";
    }

    private static string? DetectRiskyProfiles(BluetoothDevice device)
    {
        var riskyProfiles = device.Profiles
            .Where(profile => IsRiskyProfile(profile) && !IsAudioProfile(profile))
            .Select(profile => $"{profile.Name} ({profile.Uuid})")
            .ToList();

        return riskyProfiles.Count == 0
            ? null
            : $"Найдены рискованные Bluetooth profiles: {string.Join(", ", riskyProfiles)}.";
    }

    private static bool IsProbablyAudio(BluetoothDevice device)
    {
        var major = GetMajorDeviceClass(device);
        return major == MajorDeviceAudioVideo
               || ContainsAny(device.Class, "audio", "headset", "headphones")
               || ContainsAny(device.Name, "headset", "headphone", "earbuds", "audio", "buds", "наушник", "гарнитур");
    }

    private static bool IsRiskyProfile(BluetoothProfile profile)
        => ContainsAny(profile.Name, "network", "nap", "pan", "serial", "obex", "object push", "file transfer")
           || IsAssignedUuid(profile, "1101", "1105", "1106", "1115", "1116", "1117");

    private static bool IsAudioProfile(BluetoothProfile profile)
        => ContainsAny(profile.Name, "audio", "headset", "handsfree", "a2dp", "avrcp")
           || IsAssignedUuid(profile, "1108", "110B", "110C", "110D", "111E");

    private static bool IsAssignedUuid(BluetoothProfile profile, params string[] shortUuids)
        => shortUuids.Any(shortUuid =>
            profile.Uuid.Contains(shortUuid, StringComparison.OrdinalIgnoreCase));

    private static bool HasService(BluetoothDevice device, uint serviceMask)
        => (device.ClassOfDevice & serviceMask) != 0;

    private static uint GetMajorDeviceClass(BluetoothDevice device)
        => (device.ClassOfDevice >> 8) & 0x1F;

    private static string FormatDevice(BluetoothDevice device)
        => $"{device.Name}, address={device.Address}, class={device.Class}, cod=0x{device.ClassOfDevice:X6}";

    private delegate string? BluetoothRule(BluetoothDevice device);
}
