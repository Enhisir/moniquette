using Hardware.Info;

namespace Moniquette.Common.Models;

public class HardwareBriefInfo
{
    public OS OperatingSystem { get; private set; } = null!;
  public MemoryStatus MemoryStatus { get; private set; } = null!;
  public List<Battery> BatteryList { get; private set; } = null!;
  public List<BIOS> BiosList { get; private set; }= null!;
  public List<ComputerSystem> ComputerSystemList { get; private set; } = null!;
  public List<CPU> CpuList { get; private set; } = null!;
  public List<Drive> DriveList { get; private set; } = null!;
  public List<Keyboard> KeyboardList { get; private set; } = null!;
  public List<Memory> MemoryList { get; private set; } = null!;
  public List<Hardware.Info.Monitor> MonitorList { get; private set; } = null!;
  public List<Motherboard> MotherboardList { get; private set; } = null!;
  public List<Mouse> MouseList { get; private set; } = null!;
  public List<Printer> PrinterList { get; private set; } = null!;
  public List<SoundDevice> SoundDeviceList { get; private set; } = null!;
  public List<VideoController> VideoControllerList { get; private set; } = null!;

  public static HardwareBriefInfo FromFullInfo(IHardwareInfo info) 
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