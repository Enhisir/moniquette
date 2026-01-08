export type Hardware = {
    operatingSystem: string
    availableRam: number
    // battery
    bios?: BIOS
    computerSystem?: ComputerSystem
    cpu: CPU
    motherboard: Motherboard
    mouseList: Mouse[]
    soundDeviceList: SoundDevice[]
}

type BIOS = {
    manufacturer?: string
    version: number
}

type ComputerSystem = {
    name?: string
}

type CPU = {
    name?: string
    cores: number
    numberOfLogicalProcessors: number
    l1InstructionCacheSize: number
    l1DataCacheSize: number
    l2CacheSize: number
    l3CacheSize: number
}

type Motherboard = {
    manufacturer?: string
    product?: string
}

type Mouse = {
    name?: string
    manufacturer?: string
}

type OS = {
    name?: string
    versionString?: string
}

type SoundDevice = {
    name?: string
    manufacturer?: string
}