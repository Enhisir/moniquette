namespace Moniquette.Common.Models.Hardware;

public class UsbInterface
{
    public int Id { get; set; }

    public byte ClassCode { get; set; }
    
    public byte HidProtocol { get; set; }
}