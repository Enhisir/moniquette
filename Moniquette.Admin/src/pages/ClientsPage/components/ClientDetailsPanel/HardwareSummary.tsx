import type {
  Battery,
  BIOS,
  BluetoothDevice,
  CPU,
  ComputerSystem,
  HardwareBriefInfo,
  Motherboard,
  Mouse,
  SoundDevice,
  UsbDevice,
} from "@/entities/hardware/types";

type Props = {
  hardware: HardwareBriefInfo;
};

export const HardwareSummary = ({ hardware }: Props) => {
  return (
    <div className="mt-6 flex flex-col gap-4">
      <Section title="Общая информация">
        <InfoGrid
          items={[
            ["Операционная система", hardware.operatingSystem],
            ["Доступная ОЗУ", formatBytesToGb(hardware.availableRam)],
          ]}
        />
      </Section>

      {hardware.battery && <BatterySection battery={hardware.battery} />}
      {hardware.bios && <BiosSection bios={hardware.bios} />}
      {hardware.computerSystem && (
        <ComputerSystemSection computerSystem={hardware.computerSystem} />
      )}
      {hardware.cpu && <CpuSection cpu={hardware.cpu} />}
      {hardware.motherboard && (
        <MotherboardSection motherboard={hardware.motherboard} />
      )}

      <MouseListSection mouseList={hardware.mouseList} />
      <SoundDeviceListSection soundDeviceList={hardware.soundDeviceList} />
      <UsbDeviceListSection usbDevices={hardware.usbDevices} />
      <BluetoothDeviceListSection bluetoothDevices={hardware.bluetoothDevices} />
    </div>
  );
};

const Section = ({
  title,
  children,
}: {
  title: string;
  children: React.ReactNode;
}) => (
  <section className="rounded-lg border border-gray-200 bg-gray-100 p-4">
    <h3 className="mb-3 text-sm font-semibold text-black">{title}</h3>
    {children}
  </section>
);

const InfoGrid = ({
  items,
}: {
  items: Array<[string, string | number | undefined | null]>;
}) => (
  <div className="grid gap-2 md:grid-cols-2">
    {items.map(([label, value]) => (
      <div
        key={label}
        className="rounded-md border border-gray-200 bg-white px-3 py-2"
      >
        <div className="text-[11px] uppercase tracking-wide text-gray-500">
          {label}
        </div>
        <div className="mt-1 text-sm text-black">{formatValue(value)}</div>
      </div>
    ))}
  </div>
);

const BatterySection = ({ battery }: { battery: Battery }) => {
  const wearPercent =
    battery.designCapacity > 0
      ? Math.max(
          0,
          100 - (battery.fullChargeCapacity / battery.designCapacity) * 100
        )
      : 0;

  return (
    <Section title="Батарея">
      <InfoGrid
        items={[
          ["Полная ёмкость", battery.fullChargeCapacity],
          ["Проектная ёмкость", battery.designCapacity],
          ["Износ", `${wearPercent.toFixed(1)} %`],
        ]}
      />
    </Section>
  );
};

const BiosSection = ({ bios }: { bios: BIOS }) => (
  <Section title="BIOS">
    <InfoGrid
      items={[
        ["Производитель", bios.manufacturer],
        ["Версия", bios.version],
      ]}
    />
  </Section>
);

const ComputerSystemSection = ({
  computerSystem,
}: {
  computerSystem: ComputerSystem;
}) => (
  <Section title="Система">
    <InfoGrid
      items={[
        ["Модель", computerSystem.name],
        ["Производитель", computerSystem.vendor],
      ]}
    />
  </Section>
);

const CpuSection = ({ cpu }: { cpu: CPU }) => (
  <Section title="Процессор">
    <InfoGrid
      items={[
        ["Название", cpu.name],
        ["Ядра", cpu.cores],
        ["Потоки", cpu.numberOfLogicalProcessors],
        ["L1 Instruction Cache", formatBytes(cpu.l1InstructionCacheSize)],
        ["L1 Data Cache", formatBytes(cpu.l1DataCacheSize)],
        ["L2 Cache", formatBytes(cpu.l2CacheSize)],
        ["L3 Cache", formatBytes(cpu.l3CacheSize)],
      ]}
    />
  </Section>
);

const MotherboardSection = ({
  motherboard,
}: {
  motherboard: Motherboard;
}) => (
  <Section title="Материнская плата">
    <InfoGrid
      items={[
        ["Производитель", motherboard.manufacturer],
        ["Модель", motherboard.product],
      ]}
    />
  </Section>
);

const MouseListSection = ({ mouseList }: { mouseList: Mouse[] }) => (
  <Section title="Устройства ввода">
    <SimpleDeviceList
      emptyText="Мыши не обнаружены"
      items={mouseList.map((mouse, index) => ({
        key: `${mouse.name}-${index}`,
        title: mouse.name,
        subtitle: mouse.manufacturer,
      }))}
    />
  </Section>
);

const SoundDeviceListSection = ({
  soundDeviceList,
}: {
  soundDeviceList: SoundDevice[];
}) => (
  <Section title="Звуковые устройства">
    <SimpleDeviceList
      emptyText="Звуковые устройства не обнаружены"
      items={soundDeviceList.map((device, index) => ({
        key: `${device.name}-${index}`,
        title: device.name,
        subtitle: device.manufacturer,
      }))}
    />
  </Section>
);

const UsbDeviceListSection = ({ usbDevices }: { usbDevices: UsbDevice[] }) => (
  <Section title="USB-устройства">
    {usbDevices.length === 0 ? (
      <EmptyState text="USB-устройства не обнаружены" />
    ) : (
      <div className="flex flex-col gap-3">
        {usbDevices.map((device, index) => (
          <div
            key={`${device.name}-${index}`}
            className="rounded-md border border-gray-200 bg-white p-3"
          >
            <div className="text-sm font-medium text-black">{device.name}</div>

            <div className="mt-2 flex flex-wrap gap-2">
              {device.interfaces.length === 0 ? (
                <span className="text-xs text-gray-500">
                  Интерфейсы не указаны
                </span>
              ) : (
                device.interfaces.map((usbInterface) => (
                  <span
                    key={`${device.name}-${usbInterface.id}`}
                    className="rounded-full bg-blue-100 px-2 py-1 text-xs text-blue-700"
                  >
                    #{usbInterface.id} {usbInterface.typeString}
                  </span>
                ))
              )}
            </div>
          </div>
        ))}
      </div>
    )}
  </Section>
);

const BluetoothDeviceListSection = ({
  bluetoothDevices,
}: {
  bluetoothDevices: BluetoothDevice[];
}) => (
  <Section title="Bluetooth-устройства">
    {bluetoothDevices.length === 0 ? (
      <EmptyState text="Bluetooth-устройства не обнаружены" />
    ) : (
      <div className="flex flex-col gap-3">
        {bluetoothDevices.map((device, index) => (
          <div
            key={`${device.address}-${index}`}
            className="rounded-md border border-gray-200 bg-white p-3"
          >
            <div className="flex flex-wrap items-center gap-2">
              <div className="text-sm font-medium text-black">{device.name}</div>
              <span
                className={`rounded-full px-2 py-1 text-xs ${
                  device.connected
                    ? "bg-green-100 text-green-700"
                    : "bg-gray-200 text-gray-600"
                }`}
              >
                {device.connected ? "Подключено" : "Не подключено"}
              </span>
              <span className="rounded-full bg-gray-100 px-2 py-1 text-xs text-gray-700">
                {device.type}
              </span>
            </div>

            <div className="mt-2 text-xs text-gray-500">
              MAC: {device.address}
            </div>

            <div className="mt-3">
              <div className="mb-2 text-xs font-medium text-gray-700">
                Профили
              </div>

              {device.profiles.length === 0 ? (
                <span className="text-xs text-gray-500">
                  Профили не указаны
                </span>
              ) : (
                <div className="flex flex-col gap-2">
                  {device.profiles.map((profile) => (
                    <div
                      key={`${device.address}-${profile.uuid}`}
                      className="rounded border border-gray-200 bg-gray-50 px-3 py-2"
                    >
                      <div className="text-sm text-black">{profile.name}</div>
                      <div className="mt-1 break-all font-mono text-[11px] text-gray-500">
                        {profile.uuid}
                      </div>
                    </div>
                  ))}
                </div>
              )}
            </div>
          </div>
        ))}
      </div>
    )}
  </Section>
);

const SimpleDeviceList = ({
  items,
  emptyText,
}: {
  items: Array<{
    key: string;
    title?: string;
    subtitle?: string;
  }>;
  emptyText: string;
}) => {
  if (items.length === 0) {
    return <EmptyState text={emptyText} />;
  }

  return (
    <div className="flex flex-col gap-2">
      {items.map((item) => (
        <div
          key={item.key}
          className="rounded-md border border-gray-200 bg-white px-3 py-2"
        >
          <div className="text-sm text-black">{formatValue(item.title)}</div>
          {item.subtitle ? (
            <div className="mt-1 text-xs text-gray-500">{item.subtitle}</div>
          ) : null}
        </div>
      ))}
    </div>
  );
};

const EmptyState = ({ text }: { text: string }) => (
  <div className="rounded-md border border-dashed border-gray-300 bg-white px-3 py-4 text-sm text-gray-500">
    {text}
  </div>
);

const formatValue = (value: string | number | undefined | null) => {
  if (value === undefined || value === null || value === "") {
    return "Не указано";
  }

  return String(value);
};

const formatBytesToGb = (bytes: number) => `${(bytes / 1024 ** 3).toFixed(1)} ГБ`;

const formatBytes = (bytes: number) => {
  if (bytes >= 1024 ** 2) {
    return `${(bytes / 1024 ** 2).toFixed(1)} МБ`;
  }

  if (bytes >= 1024) {
    return `${(bytes / 1024).toFixed(1)} КБ`;
  }

  return `${bytes} Б`;
};