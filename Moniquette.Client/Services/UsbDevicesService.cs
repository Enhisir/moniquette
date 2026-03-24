using LibUsbDotNet.Main;
using Moniquette.Client.Services.Abstractions;
using Moniquette.Common.Models.Hardware;
using UsbDevice = Moniquette.Common.Models.Hardware.UsbDevice;

namespace Moniquette.Client.Services;

public class UsbDevicesService : IUsbDevicesService
{
    public List<UsbDevice> GetAttached()
    {
        var deviceList = LibUsbDotNet.UsbDevice.AllDevices;

        var attachedUsbDevices = new List<UsbDevice>();
        foreach (UsbRegistry registry in deviceList)
        {
            var usb = GetDeviceFromRegistry(registry);
            if (usb is not null)
            {
                attachedUsbDevices.Add(usb);
            }
        }

        return attachedUsbDevices;
    }

    private UsbDevice? GetDeviceFromRegistry(UsbRegistry registry)
    {
        if (!registry.Open(out var device)) return null;
        try
        {
            var usb = new UsbDevice
            {
                Name = registry.Name,
                Interfaces = device.Configs
                    .SelectMany(cfg => cfg.InterfaceInfoList)
                    .Select(inf => new UsbInterface
                    {
                        Id = inf.Descriptor.InterfaceID,
                        TypeString = Enum.GetName(inf.Descriptor.Class)?.ToLowerInvariant() ?? "unknown"
                    })
                    .ToList()
            };
            return usb;
        }
        catch (Exception e)
        {
            // Log Message
            // Console.WriteLine(e);
            // throw;
            return null;
        }
        finally
        {
            device.Close();
        }
    }
}