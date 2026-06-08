namespace Moniquette.Common.Models.Hardware;

public class BluetoothDevice
{
    public string Address { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Class { get; set; } = null!;

    public uint ClassOfDevice { get; set; }

    public List<BluetoothProfile> Profiles { get; } = [];
}

public class BluetoothProfile
{
    public string Name { get; set; } = "";

    public string Uuid { get; set; } = "";
}
