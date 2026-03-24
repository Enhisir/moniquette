using Moniquette.Common.Models.Hardware;

namespace Moniquette.Client.Services.Abstractions;

public interface IUsbDevicesService
{
    public List<UsbDevice> GetAttached();
}