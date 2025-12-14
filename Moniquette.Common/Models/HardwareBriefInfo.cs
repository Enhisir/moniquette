using HWInfo = Hardware.Info;

namespace Moniquette.Common.Models;

public class HardwareBriefInfo
{
    public HWInfo.OS OperatingSystem { get; set; } = null!;
    public HWInfo.MemoryStatus MemoryStatus { get; set; } = null!;
    public List<HWInfo.Battery> BatteryList { get; set; } = null!;
    public List<HWInfo.BIOS> BiosList { get; set; } = null!;
    public List<HWInfo.ComputerSystem> ComputerSystemList { get; set; } = null!;
    public List<HWInfo.CPU> CpuList { get; set; } = null!;
    public List<HWInfo.Drive> DriveList { get; set; } = null!;
    public List<HWInfo.Keyboard> KeyboardList { get; set; } = null!;
    public List<HWInfo.Memory> MemoryList { get; set; } = null!;
    public List<HWInfo.Monitor> MonitorList { get; set; } = null!;
    public List<HWInfo.Motherboard> MotherboardList { get; set; } = null!;
    public List<HWInfo.Mouse> MouseList { get; set; } = null!;
    public List<HWInfo.Printer> PrinterList { get; set; } = null!;
    public List<HWInfo.SoundDevice> SoundDeviceList { get; set; } = null!;
    public List<HWInfo.VideoController> VideoControllerList { get; set; } = null!;

    public static HardwareBriefInfo FromFullInfo(HWInfo.IHardwareInfo info)
        => new()
        {
            OperatingSystem = info.OperatingSystem,
            MemoryStatus = info.MemoryStatus,
            BatteryList = info.BatteryList,
            BiosList = info.BiosList,
            ComputerSystemList = info.ComputerSystemList,
            CpuList = info.CpuList,
            DriveList = info.DriveList,
            KeyboardList = info.KeyboardList,
            MemoryList = info.MemoryList,
            MonitorList = info.MonitorList,
            MotherboardList = info.MotherboardList,
            MouseList = info.MouseList,
            PrinterList = info.PrinterList,
            SoundDeviceList = info.SoundDeviceList,
            VideoControllerList = info.VideoControllerList
        };
}

#region Optimized code version
/*
using Moniquette.Common.Models.Hardware;
using HWInfo = Hardware.Info;

namespace Moniquette.Common.Models;

public class HardwareBriefInfo
{
    public string OperatingSystem { get; set; } = null!;
    public ulong AvailableRAM { get; set; } // Physical RAM in bytes
    public Battery? Battery { get; set; } = null!; // заменить на мою батарею
    public BIOS? Bios { get; set; } = null!;
    public ComputerSystem? ComputerSystem { get; set; } = null!;
    public CPU? CpuList { get; set; } = null!;
    // public List<HWInfo.Memory> MemoryList { get; private set; } = null!; // переписать
    public Motherboard? MotherboardList { get; set; } = null!;
    public List<Mouse> MouseList { get; set; } = null!;
    public List<SoundDevice> SoundDeviceList { get; set; } = null!;
    
    // не думаю, что можно будет адекватно выделять странные устройства из GPU
    // public List<VideoController> VideoControllerList { get; private set; } = null!;

    public static HardwareBriefInfo FromFullInfo(HWInfo.IHardwareInfo info)
        => new()
        {
            OperatingSystem = $"{info.OperatingSystem.Name} {info.OperatingSystem.VersionString}",
            AvailableRAM = info.MemoryStatus.TotalPhysical,
            Battery = info
                .BatteryList
                .Select(b => new Battery { FullChargeCapacity = b.FullChargeCapacity, DesignCapacity = b.DesignCapacity })
                .FirstOrDefault(), // батарей может быть больше, чем одна (?), но этот факт малополезен
            Bios = info
                .BiosList
                .Select(bi => new BIOS { Manufacturer = bi.Manufacturer, Version = bi.Version })
                .SingleOrDefault(),
            ComputerSystem = info
                .ComputerSystemList
                .Select(s => new ComputerSystem { Name = s.Name, Vendor = s.Vendor})
                .SingleOrDefault(),
            CpuList = info
                .CpuList
                .Select(cp=> new CPU {
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
            MotherboardList = info
                .MotherboardList
                .Select(mb => new Motherboard {  Manufacturer = mb.Manufacturer, Product = mb.Product })
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
*/
#endregion