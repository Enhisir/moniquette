using Moniquette.Common.Enums;
using Moniquette.Common.Models;
using Moniquette.Common.Models.Hardware;

namespace Moniquette.Server.Analysis;

public class UsbDeviceAnalyzer : AnalyzerBase
{
    private const byte UseInterfaceDescriptors = 0x00;
    private const byte AudioClass = 0x01;
    private const byte CommunicationsClass = 0x02;
    private const byte HidClass = 0x03;
    private const byte StillImageClass = 0x06;
    private const byte MassStorageClass = 0x08;
    private const byte CdcDataClass = 0x0A;
    private const byte WirelessControllerClass = 0xE0;
    private const byte MiscellaneousClass = 0xEF;
    private const byte VendorSpecificClass = 0xFF;

    private const byte HidBootInterfaceSubClass = 0x01;
    private const byte HidKeyboardProtocol = 0x01;
    private const byte HidMouseProtocol = 0x02;

    private static readonly IReadOnlyCollection<UsbRule> Rules =
    [
        DetectPhoneLikeCompositeDevice,
        DetectCommunicationsDevice,
        DetectCdcDataDevice,
        DetectStillImageOrMtpDevice,
        DetectMassStorageDevice,
        // Отключено как слишком широкое правило: сюда попадают обычные USB-адаптеры мышей/клавиатур.
        // DetectWirelessControllerDevice,
        // Отключено как слишком широкое правило: отдельная клавиатура сама по себе не достаточный признак риска.
        // DetectKeyboardHidDevice,
        DetectVendorSpecificMobileDevice
    ];

    public override Task<IReadOnlyCollection<Threat>> AnalyzeAsync(
        Report report,
        CancellationToken cancellationToken)
    {
        var threats = report.HardwareInfo.UsbDevices
            .SelectMany(device => Rules
                .Select(rule => rule(device))
                .Where(reason => !string.IsNullOrWhiteSpace(reason))
                .Select(reason => CreateThreat(
                    report,
                    ThreatType.Warning,
                    $"Подозрительное USB-устройство: {FormatDevice(device)}. {reason}")))
            .ToList();

        return Task.FromResult<IReadOnlyCollection<Threat>>(threats);
    }

    private static string? DetectPhoneLikeCompositeDevice(UsbDevice device)
    {
        if (IsAudioDevice(device))
        {
            return null;
        }

        if (device.Class == MiscellaneousClass
            && device.SubClass == 0x02
            && device.Protocol == 0x01)
        {
            return "Composite/IAD class 0xEF/0x02/0x01 часто используется сложными устройствами, включая смартфоны.";
        }

        return null;
    }

    private static string? DetectCommunicationsDevice(UsbDevice device)
    {
        if (HasClass(device, CommunicationsClass))
        {
            return "Найден USB Communications/CDC class 0x02: устройство может предоставлять модемный, сетевой или serial-интерфейс.";
        }

        return null;
    }

    private static string? DetectCdcDataDevice(UsbDevice device)
    {
        if (HasClass(device, CdcDataClass))
        {
            return "Найден USB CDC Data class 0x0A: устройство может предоставлять data-интерфейс телефона, модема или сетевого адаптера.";
        }

        return null;
    }

    private static string? DetectStillImageOrMtpDevice(UsbDevice device)
    {
        if (HasClass(device, StillImageClass))
        {
            return "Найден USB Still Image class 0x06: этот класс используется PTP/MTP-устройствами, включая телефоны и камеры.";
        }

        return null;
    }

    private static string? DetectMassStorageDevice(UsbDevice device)
    {
        if (HasClass(device, MassStorageClass))
        {
            return "Найден USB Mass Storage class 0x08: подключен внешний накопитель или устройство в режиме накопителя.";
        }

        return null;
    }

    private static string? DetectWirelessControllerDevice(UsbDevice device)
    {
        if (HasClass(device, WirelessControllerClass))
        {
            return "Найден USB Wireless Controller class 0xE0: устройство может быть Bluetooth/Wireless контроллером.";
        }

        return null;
    }

    private static string? DetectKeyboardHidDevice(UsbDevice device)
    {
        var keyboardInterface = device.Interfaces?.FirstOrDefault(i =>
            i.ClassCode == HidClass
            && i.SubClass == HidBootInterfaceSubClass
            && i.Protocol == HidKeyboardProtocol);

        if (keyboardInterface is null)
        {
            return null;
        }

        return $"Найден USB HID boot keyboard interface #{keyboardInterface.Id}: дополнительная клавиатура может быть техническим признаком риска.";
    }

    private static string? DetectVendorSpecificMobileDevice(UsbDevice device)
    {
        if (!HasClass(device, VendorSpecificClass))
        {
            return null;
        }

        if (ContainsAny(device.Name, "phone", "android", "iphone", "mtp", "adb", "смартфон", "телефон"))
        {
            return "Найден vendor-specific USB class 0xFF с признаками мобильного устройства в имени.";
        }

        var androidDebugBridge = device.Interfaces?.FirstOrDefault(i =>
            i.ClassCode == VendorSpecificClass
            && i.SubClass == 0x42
            && i.Protocol == 0x01);

        return androidDebugBridge is null
            ? null
            : $"Найден vendor-specific Android Debug Bridge interface #{androidDebugBridge.Id}: class/subclass/protocol 0xFF/0x42/0x01.";
    }

    private static bool HasClass(UsbDevice device, byte classCode)
    {
        if (device.Class == classCode)
        {
            return true;
        }

        if (device.Class != UseInterfaceDescriptors && device.Interfaces is { Count: > 0 })
        {
            return false;
        }

        return device.Interfaces?.Any(i => i.ClassCode == classCode) == true;
    }

    private static bool IsAudioDevice(UsbDevice device)
        => HasClass(device, AudioClass)
           || ContainsAny(device.Name, "headset", "headphone", "earbuds", "audio", "наушник", "гарнитур");

    private static string FormatDevice(UsbDevice device)
        => $"{device.Name}, device=0x{device.Class:X2}/0x{device.SubClass:X2}/0x{device.Protocol:X2}, vendor=0x{device.VendorId:X4}, product=0x{device.ProductId:X4}";

    private delegate string? UsbRule(UsbDevice device);
}
