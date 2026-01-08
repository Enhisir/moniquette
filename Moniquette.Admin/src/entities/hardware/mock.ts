import type { Hardware } from "./types";

export const msiKatana17HardwareMock: Hardware = {
  operatingSystem: "Debian GNU/Linux 12",
  availableRam: 34_359_738_368,

  bios: {
    manufacturer: "American Megatrends International, LLC.",
    version: 1.12,
  },

  computerSystem: {
    name: "MSI Katana 17 B12VGK",
  },

  cpu: {
    name: "Intel Core i7-12700H",
    cores: 14,
    numberOfLogicalProcessors: 20,
    l1InstructionCacheSize: 640,
    l1DataCacheSize: 768,
    l2CacheSize: 12288,
    l3CacheSize: 24576,
  },

  motherboard: {
    manufacturer: "Micro-Star International Co., Ltd.",
    product: "MS-1583",
  },

  mouseList: [
    {
      name: "HID-compliant mouse",
      manufacturer: "Microsoft",
    },
  ],

  soundDeviceList: [
    {
      name: "Realtek High Definition Audio",
      manufacturer: "Realtek Semiconductor Corp.",
    },
    {
      name: "NVIDIA High Definition Audio",
      manufacturer: "NVIDIA",
    },
  ],
};

export const virtualBoxHardwareMock: Hardware = {
  operatingSystem: "Debian GNU/Linux 12",
  availableRam: 4_294_967_296,

  bios: {
    manufacturer: "Phoenix Technologies LTD",
    version: 1.0,
  },

  computerSystem: {
    name: "VirtualBox VM",
  },

  cpu: {
    name: "Intel Core i7 (Virtual CPU)",
    cores: 2, // Обычно VM выдаёт меньше ядер
    numberOfLogicalProcessors: 2,
    l1InstructionCacheSize: 32,
    l1DataCacheSize: 32,
    l2CacheSize: 256,
    l3CacheSize: 4096,
  },

  motherboard: {
    manufacturer: "Oracle Corporation",
    product: "VirtualBox",
  },

  mouseList: [
    {
      name: "PS/2 Compatible Mouse",
      manufacturer: "VirtualBox",
    },
  ],

  soundDeviceList: [
    {
      name: "Intel HD Audio",
      manufacturer: "VirtualBox",
    },
  ],
};
