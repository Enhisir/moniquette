using Moniquette.Common.Models.Hardware;
using HWInfo = Hardware.Info;

namespace Moniquette.Common.Models;

public class HardwareBriefInfo
{
    public string OperatingSystem { get; set; } = null!;
    public ulong AvailableRam { get; set; } // Physical RAM in bytes
    public Battery? Battery { get; set; }
    public BIOS? Bios { get; set; }
    public ComputerSystem? ComputerSystem { get; set; }

    public CPU? Cpu { get; set; }

    // public List<HWInfo.Memory> MemoryList { get; private set; } = null!; // переписать
    public Motherboard? Motherboard { get; set; }
    public List<Mouse> MouseList { get; set; } = null!;
    public List<SoundDevice> SoundDeviceList { get; set; } = null!;
    public List<UsbDevice> UsbDevices { get; set; } = null!;

    public List<BluetoothDevice> BluetoothDevices { get; set; } = null!;

    public static HardwareBriefInfo FromFullInfo(HWInfo.IHardwareInfo info)
        => new()
        {
            OperatingSystem = $"{info.OperatingSystem.Name} {info.OperatingSystem.VersionString}",
            AvailableRam = info.MemoryStatus.TotalPhysical,
            Battery = info
                .BatteryList
                .Select(b => new Battery
                    { FullChargeCapacity = b.FullChargeCapacity, DesignCapacity = b.DesignCapacity })
                .FirstOrDefault(), // батарей может быть больше, чем одна (?), но этот факт малополезен
            Bios = info
                .BiosList
                .Select(bi => new BIOS { Manufacturer = bi.Manufacturer, Version = bi.Version })
                .SingleOrDefault(),
            ComputerSystem = info
                .ComputerSystemList
                .Select(s => new ComputerSystem { Name = s.Name, Vendor = s.Vendor })
                .SingleOrDefault(),
            Cpu = info
                .CpuList
                .Select(cp => new CPU
                {
                    Name = cp.Name,
                    Cores = cp.NumberOfCores,
                    NumberOfLogicalProcessors = cp.NumberOfLogicalProcessors,
                    L1DataCacheSize = cp.L1DataCacheSize,
                    L1InstructionCacheSize = cp.L1InstructionCacheSize,
                    L2CacheSize = cp.L2CacheSize,
                    L3CacheSize = cp.L3CacheSize
                })
                .SingleOrDefault(), // CPU не должно быть больше одного на клиентском устройстве
            // MemoryList = info.MemoryList,
            Motherboard = info
                .MotherboardList
                .Select(mb => new Motherboard { Manufacturer = mb.Manufacturer, Product = mb.Product })
                .SingleOrDefault(), // Материнских плат не должно быть больше одной на клиентском устройстве
            MouseList = info
                .MouseList
                .Select(ms => new Mouse { Name = ms.Name, Manufacturer = ms.Manufacturer })
                .ToList(),
            SoundDeviceList = info
                .SoundDeviceList
                .Select(sd => new SoundDevice { Name = sd.Name, Manufacturer = sd.Manufacturer })
                .ToList()
        };
}