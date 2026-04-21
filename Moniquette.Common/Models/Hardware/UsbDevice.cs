namespace Moniquette.Common.Models.Hardware;

public class UsbDevice
{
    public string Name { get; set; } = null!;
    
    public ushort VendorId { get; set; }
    
    public ushort ProductId { get; set; }
    
    public byte Class { get; set; }

    public List<UsbInterface> Interfaces { get; set; } = null!;
}