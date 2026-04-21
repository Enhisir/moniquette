using LibUsbDotNet.LibUsb;
using Moniquette.Client.Services.Abstractions;
using Moniquette.Common.Models.Hardware;
using UsbDevice = Moniquette.Common.Models.Hardware.UsbDevice;

namespace Moniquette.Client.Services;

public class UsbDevicesService : IUsbDevicesService
{
    public List<UsbDevice> GetAttached()
    {
        using var context = new UsbContext();
        var deviceList = context.List();

        var attachedUsbDevices = new List<UsbDevice>();
        foreach (var device in deviceList)
        {
            var usb = new UsbDevice
            {
                Name = $"{device.Info.Manufacturer} {device.Info.Product}", // TODO: добавить имя, на Linux не работает
                VendorId = device.VendorId,
                ProductId = device.ProductId,
                Class = device.Info.DeviceClass,
                Interfaces = device.Configs
                    .SelectMany(cfg => cfg.Interfaces)
                    .Select(uIntInfo => new UsbInterface
                    {
                        Id = uIntInfo.Number,
                        ClassCode = (byte) uIntInfo.Class,
                        HidProtocol = uIntInfo.Protocol 
                    })
                    .ToList()
            };
            attachedUsbDevices.Add(usb);
        }

        return attachedUsbDevices;
    }
}