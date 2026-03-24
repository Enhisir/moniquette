using Moniquette.Client.Services.Abstractions;
using Moniquette.Common.Models;

namespace Moniquette.Client.Pipeline.Fillers;

public class HardwareFiller(
    IUsbDevicesService usbService,
    IBluetoothDevicesService bluetoothService,
    Hardware.Info.IHardwareInfo info) : IReportFiller
{
    public async Task<Report> Fill(Report report, CancellationToken cancellationToken)
    {
        // getting info from IHardwareInfo is slightly connected with delays,
        // at least at first run. That's why it's wrapped into await call
        await Task.Run(info.RefreshAll, cancellationToken); 
        report.HardwareInfo = HardwareBriefInfo.FromFullInfo(info);
        report.HardwareInfo.UsbDevices = usbService.GetAttached();
        report.HardwareInfo.BluetoothDevices = bluetoothService.GetAttached();
        return report;
    }
}