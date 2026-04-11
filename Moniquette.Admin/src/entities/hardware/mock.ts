import type { HardwareBriefInfo } from "./types";

export const msiKatana17HardwareMock: HardwareBriefInfo = {
  operatingSystem: "Debian GNU/Linux 12",
  availableRam: 33338351616,
  battery: {
    fullChargeCapacity: 2926000,
    designCapacity: 4562000,
  },
  bios: {
    manufacturer: "American Megatrends International, LLC.",
    version: "E17L5IMS.308",
  },
  computerSystem: {
    name: "Katana 17 B12VGK",
    vendor: "Micro-Star International Co., Ltd.",
  },
  cpu: {
    name: "12th Gen Intel(R) Core(TM) i7-12650H",
    cores: 10,
    numberOfLogicalProcessors: 16,
    l1InstructionCacheSize: 32768,
    l1DataCacheSize: 49152,
    l2CacheSize: 1310720,
    l3CacheSize: 25165824,
  },
  motherboard: {
    manufacturer: "Micro-Star International Co., Ltd.",
    product: "MS-17L5",
  },
  mouseList: [
    {
      name: "MSNB0001:00 04F3:30AA Mouse",
      manufacturer: "",
    },
  ],
  soundDeviceList: [
    {
      name: "Intel Corporation Alder Lake PCH-P High Definition Audio Controller (rev 01)",
      manufacturer: "",
    },
  ],
  usbDevices: [],
  bluetoothDevices: [
    {
      address: "AC:80:0A:DC:44:F2",
      name: "WH-1000XM5",
      type: "unknown",
      connected: true,
      profiles: [
        {
          name: "Vendor specific",
          uuid: "00000000-deca-fade-deca-deafdecacaff",
        },
        {
          name: "Headset",
          uuid: "00001108-0000-1000-8000-00805f9b34fb",
        },
        {
          name: "Audio Sink",
          uuid: "0000110b-0000-1000-8000-00805f9b34fb",
        },
        {
          name: "A/V Remote Control Target",
          uuid: "0000110c-0000-1000-8000-00805f9b34fb",
        },
        {
          name: "Advanced Audio Distribution",
          uuid: "0000110d-0000-1000-8000-00805f9b34fb",
        },
      ],
    },
  ],
};

export const virtualBoxHardwareMock: HardwareBriefInfo = {
  operatingSystem: "Microsoft Windows 10 Pro",
  availableRam: 2147483648,
  battery: undefined,
  bios: {
    manufacturer: "innotek GmbH",
    version: "VirtualBox",
  },
  computerSystem: {
    name: "VirtualBox",
    vendor: "Oracle Corporation",
  },
  cpu: {
    name: "Intel(R) Core(TM) i5 Virtual CPU",
    cores: 2,
    numberOfLogicalProcessors: 2,
    l1InstructionCacheSize: 32768,
    l1DataCacheSize: 32768,
    l2CacheSize: 262144,
    l3CacheSize: 0,
  },
  motherboard: {
    manufacturer: "Oracle Corporation",
    product: "VirtualBox",
  },
  mouseList: [
    {
      name: "Virtual USB Tablet",
      manufacturer: "Oracle",
    },
  ],
  soundDeviceList: [
    {
      name: "Virtual Audio Device",
      manufacturer: "Oracle",
    },
  ],
  usbDevices: [
    {
      name: "Android Phone",
      interfaces: [
        { id: 0, typeString: "MTP" },
        { id: 1, typeString: "ADB" },
      ],
    },
    {
      name: "VirtualBox USB",
      interfaces: [{ id: 0, typeString: "Generic USB" }],
    },
  ],
  bluetoothDevices: [
    {
      address: "34:AB:12:CD:EF:90",
      name: "Galaxy S24",
      type: "phone",
      connected: true,
      profiles: [
        {
          name: "Handsfree",
          uuid: "0000111e-0000-1000-8000-00805f9b34fb",
        },
        {
          name: "Phonebook Access",
          uuid: "0000112f-0000-1000-8000-00805f9b34fb",
        },
      ],
    },
  ],
};