namespace Moniquette.Common.Models.Hardware;

public class UsbDevice
{
    public string Name { get; set; } = null!;

    public List<UsbInterface> Interfaces { get; set; } = null!;
}