export type HardwareBriefInfo = {
  operatingSystem: string;
  availableRam: number; // bytes
  battery?: Battery;
  bios?: BIOS;
  computerSystem?: ComputerSystem;
  cpu?: CPU;
  motherboard?: Motherboard;
  mouseList: Mouse[];
  soundDeviceList: SoundDevice[];
  usbDevices: UsbDevice[];
  bluetoothDevices: BluetoothDevice[];
};

export type Battery = {
  fullChargeCapacity: number;
  designCapacity: number;
};

export type BIOS = {
  manufacturer: string;
  version: string;
};

export type ComputerSystem = {
  name: string;
  vendor: string;
};

export type CPU = {
  name: string;
  cores: number;
  numberOfLogicalProcessors: number;
  l1InstructionCacheSize: number;
  l1DataCacheSize: number;
  l2CacheSize: number;
  l3CacheSize: number;
};

export type Motherboard = {
  manufacturer: string;
  product: string;
};

export type Mouse = {
  name: string;
  manufacturer: string;
};

export type OS = {
  name: string;
  versionString: string;
};

export type SoundDevice = {
  name: string;
  manufacturer: string;
};

export type UsbDevice = {
  name: string;
  vendorId?: number;
  productId?: number;
  class?: number;
  subClass?: number;
  protocol?: number;
  interfaces: UsbInterface[];
};

export type UsbInterface = {
  id: number;
  classCode?: number;
  subClass?: number;
  protocol?: number;
  hidProtocol?: number;
  typeString: string;
};

export type BluetoothDevice = {
  address: string;
  name: string;
  class?: string;
  classOfDevice?: number;
  type: string;
  connected: boolean;
  profiles: BluetoothProfile[];
};

export type BluetoothProfile = {
  name: string;
  uuid: string;
};
