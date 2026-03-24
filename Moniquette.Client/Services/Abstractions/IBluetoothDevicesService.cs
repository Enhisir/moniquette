using Moniquette.Common.Models.Hardware;

namespace Moniquette.Client.Services.Abstractions;

public interface IBluetoothDevicesService
{
    public List<BluetoothDevice> GetAttached();
}